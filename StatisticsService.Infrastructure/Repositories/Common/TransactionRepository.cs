using System.Globalization;
using System.Text;
using AutoBogus;
using AutoMapper;
using ClickHouse.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using StatisticsService.Domain.Entities;
using StatisticsService.Infrastructure.Dto;
using StatisticsService.Infrastructure.Dto.Filters;
using StatisticsService.Infrastructure.Repositories.Interfaces;

namespace StatisticsService.Infrastructure.Repositories.Common;

public class TransactionRepository : ITransactionRepository
{
    private readonly IClickHouseDatabase _database;
    private readonly IMapper _mapper;
    private const int MinCountRowsForLoad = 100000;

    public TransactionRepository(IClickHouseDatabase database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public Task<IEnumerable<TransactionDto>> GetTransactions(PagingParametrs parameters)
    {
        try
        {
            _database.Open();

            var transactionsFromDb =
                _database.ExecuteSelectCommand($"select * from transactions " +
                                               $"order by TransactionNumber " +
                                               $"LIMIT {(parameters.PageNumber - 1) * parameters.PageSize},{parameters.PageSize}");

            _database.Close();
            return Task.Run(() =>
                _mapper.Map<IEnumerable<TransactionDto>>(
                    ConvertMultidimensionalArrayToTransaction(transactionsFromDb)));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    Task<bool> ITransactionSqlRepository.AddRandomTransactions()
    {
        var faker = new AutoFaker<InputTransactionDto>()
            .RuleFor(fake => fake.TransactionTypeName, fake => fake.Random.Int(1, 10).ToString())
            .RuleFor(fake => fake.PlaceName, fake => fake.Random.Int(0).ToString())
            .RuleFor(fake => fake.ApplicationName, fake => fake.Random.Int(0, 1000).ToString())
            .RuleFor(fake => fake.DeviceSerialNumber, fake => fake.Random.Int(1, 100).ToString())
            .RuleFor(fake => fake.AgentName, fake => fake.Random.Int(0).ToString())
            .RuleFor(fake => fake.ProductName, fake => fake.Random.Int(0).ToString())
            .RuleFor(fake => fake.TransactionNumber, fake => fake.Random.Long(0))
            .RuleFor(fake => fake.TransactionDate,
                fake => fake.Date.Between(new DateTime(2021, 1, 1), new DateTime(2023, 2, 1))
                    .ToString(CultureInfo.InvariantCulture))
            .RuleFor(fake => fake.TicketRegisterDate,
                fake => fake.Date.Between(new DateTime(2021, 1, 1), new DateTime(2023, 2, 1))
                    .ToString(CultureInfo.InvariantCulture))
            .RuleFor(fake => fake.TicketDateBegin,
                fake => fake.Date.Between(new DateTime(2021, 1, 1), new DateTime(2023, 2, 1))
                    .ToString(CultureInfo.InvariantCulture))
            .RuleFor(fake => fake.TicketDateBegin,
                fake => fake.Date.Between(new DateTime(2021, 1, 1), new DateTime(2023, 2, 1))
                    .ToString(CultureInfo.InvariantCulture))
            .RuleFor(fake => fake.TicketGuid, fake => fake.Random.Guid().ToString())
            .RuleFor(fake => fake.IsOnline, fake => fake.Random.Int(0, 1).ToString())
            .RuleFor(fake => fake.TicketRemainingTripsCounter, fake => fake.Random.Int(0, 300).ToString())
            .RuleFor(fake => fake.CardRefillCounter, fake => fake.Random.Int(0, 300).ToString())
            .RuleFor(fake => fake.CardUsageCounter, fake => fake.Random.Int(0, 300).ToString())
            .RuleFor(fake => fake.IsProlong, fake => fake.Random.Int(0, 1).ToString())
            .RuleFor(fake => fake.CardNumber, fake => fake.Random.Int(0).ToString())
            .RuleFor(fake => fake.CardSerialNumber, fake => fake.Random.Int(0).ToString())
            .RuleFor(fake => fake.CardBalance, fake => fake.Commerce.Price(1, 100))
            .RuleFor(fake => fake.Price, fake => fake.Commerce.Price(1, 100))
            .RuleFor(fake => fake.TicketDateEnd,
                fake => fake.Date.Between(new DateTime(2021, 1, 1), new DateTime(2023, 2, 1))
                    .ToString(CultureInfo.InvariantCulture));

        for (var s = 0; s < 10; s++)
        {
            var lstTransaction = new List<InputTransactionDto>();
            for (var i = 0; i < 100000; i++)
            {
                lstTransaction.Add(faker.Generate());
            }

            lstTransaction.DistinctBy(x => x.TransactionNumber);

            AddTransactions(lstTransaction);
        }

        return Task.Run(() => true);
    }

    public Task<TransactionDto> GetTransaction(int tranNo)
    {
        try
        {
            _database.Open();

            var transactionFromDb =
                _database.ExecuteSelectCommand($"select * from transactions " +
                                               $"where TransactionNumber = {tranNo} ");

            return Task.Run(() => _mapper.Map<TransactionDto>(transactionFromDb));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public Task<bool> AddTransactions(IEnumerable<InputTransactionDto> transactions)
    {
        try
        {
            var listValues = transactions
                .Select(value =>
                    string.Join(", ", typeof(InputTransactionDto)
                        .GetProperties()
                        .Select(x => (x.GetValue(value), x.PropertyType))
                        .ToList()
                        .Select(x => (x.PropertyType == typeof(long) || x.PropertyType == typeof(Int32))
                            ? x.Item1?.ToString()
                            : $"'{x.Item1}'")
                        .ToArray()))
                .Select(str => $"({str})")
                .ToList();

            _database.Open();
            _database.ExecuteNonQuery(
                $"INSERT INTO transactions ({string.Join(", ", new Transaction().GetType().GetProperties().Select(x => x.Name).ToArray())}) VALUES {string.Join(", ", listValues)}");

            return Task.Run(() => true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> AddTransactionsFromFile(IEnumerable<IFormFile> uploadedFiles)
    {
        foreach (var file in uploadedFiles)
        {
            var instance = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(instance);

            var inputTransactions = new List<InputTransactionDto>();
            var countRows = 0;

            using (var streamReader = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding(1251)))
            await using (var reader = new JsonTextReader(streamReader))
            {
                reader.SupportMultipleContent = true;

                var serializer = new JsonSerializer();
                while (await reader.ReadAsync())
                {
                    if (reader.TokenType != JsonToken.StartObject) continue;
                    try
                    {
                        inputTransactions.Add(serializer.Deserialize<InputTransactionDto>(reader) ??
                                              throw new InvalidOperationException());
                        countRows++;
                        if (countRows == MinCountRowsForLoad)
                        {
                            await AddTransactions(inputTransactions);
                            inputTransactions.Clear();
                            countRows = 0;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            if (inputTransactions.Count == 0) continue;
            await AddTransactions(inputTransactions);
        }

        return await Task.Run(() => true);
    }

    public Task<List<ReportTransactionPlaceDto>> GetReportTransactionPlaces()
    {
        try
        {
            _database.Open();

            var report = _database.ExecuteQueryMapping<ReportTransactionPlaceDto>
                ("select PlaceName, count(PlaceName) CountTransaction " +
                 "from statistics.transactions " +
                 "where PlaceName != 'null' " +
                 "Group By PlaceName " +
                 "order by CountTransaction desc")
                .ToList();

            return Task.Run(() => report);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public IEnumerable<TransactionDto> GetReportTransactionsForRangeDate(DataRangeFilter dataRangeFilter,
        PagingParametrs parameters)
    {
        try
        {
            _database.Open();

            var transactions = _database.ExecuteSelectCommand
            ($"select * from statistics.transactions" +
             $" where parseDateTimeBestEffort(TransactionDate) > parseDateTimeBestEffort('{dataRangeFilter.StartRange}')" +
             $" and parseDateTimeBestEffort(TransactionDate) < parseDateTimeBestEffort('{dataRangeFilter.EndRange}')" +
             $" order by TransactionNumber " +
             $"LIMIT {parameters.PageNumber * parameters.PageSize},{parameters.PageSize}");

            return _mapper.Map<List<TransactionDto>>(ConvertMultidimensionalArrayToTransaction(transactions));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public IEnumerable<TransactionDto> GetReportTransactionsWithNotProlong(PagingParametrs parameters)
    {
        try
        {
            _database.Open();

            var transactions = _database.ExecuteSelectCommand
            ($"select * from transactions" +
             " where IsProlong = '0'" +
             $" order by TransactionNumber " +
             $"LIMIT {parameters.PageNumber * parameters.PageSize},{parameters.PageSize}");

            return _mapper.Map<List<TransactionDto>>(ConvertMultidimensionalArrayToTransaction(transactions));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static IEnumerable<Transaction> ConvertMultidimensionalArrayToTransaction(IEnumerable<object[]> array)
    {
        try
        {
            var nameProp = new Transaction()
                .GetType()
                .GetProperties()
                .Select(x => x.Name)
                .ToList();

            var transactions = new List<Transaction>();
            foreach (var transactionInArray in array.ToList())
            {
                var transaction = new Transaction();
                for (var i = 0; i < transactionInArray.Length; i++)
                {
                    transaction
                        .GetType()
                        .GetProperty(nameProp[i])?
                        .SetValue(transaction, transactionInArray[i]);
                }

                transactions.Add(transaction);
            }

            return transactions;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}