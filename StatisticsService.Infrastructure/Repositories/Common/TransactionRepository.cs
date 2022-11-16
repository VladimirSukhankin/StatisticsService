using System.Text;
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

    public void Dispose()
    {
        try
        {
            _database.Open();
            _database.Close();
        }
        catch
        {
            _database.Close();
        }
    }

    public IEnumerable<TransactionDto> GetTransactions(PagingParametrs parameters)
    {
        try
        {
            _database.Open();

            var transactionsFromDb =
                _database.ExecuteSelectCommand($"select * from transactions " +
                                               $"order by TransactionNumber " +
                                               $"LIMIT {parameters.PageNumber * parameters.PageSize},{parameters.PageSize}");

            return _mapper.Map<List<TransactionDto>>(ConvertMultidimensionalArrayToTransaction(transactionsFromDb));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public TransactionDto GetTransaction(int tranNo)
    {
        try
        {
            _database.Open();

            var transactionFromDb =
                _database.ExecuteSelectCommand($"select * from transactions " +
                                               $"where TransactionNumber = {tranNo} ");
            
            return _mapper.Map<TransactionDto>(transactionFromDb);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public bool AddTransactions(IEnumerable<InputTransactionDto> transactions)
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

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> AddTransactionsFromFile(IFormFile[] uploadedFiles)
    {
        foreach (var file in uploadedFiles)
        {
            var instance = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(instance);

            var inputTransactions = new List<InputTransactionDto>();
            var countRows = 0;

            using (StreamReader streamReader = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding(1251)))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                reader.SupportMultipleContent = true;

                var serializer = new JsonSerializer();
                while (await reader.ReadAsync())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        try
                        {
                            inputTransactions.Add(serializer.Deserialize<InputTransactionDto>(reader) ??
                                                  throw new InvalidOperationException());
                            countRows++;
                            if (countRows == MinCountRowsForLoad)
                            {
                                AddTransactions(inputTransactions);
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
            }

            if (inputTransactions.Count == 0) continue;
            AddTransactions(inputTransactions);
        }

        return await Task.Run(() => true);
    }

    public IEnumerable<ReportTransactionPlaceDto> GetReportTransactionPlaces()
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

            return report;
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