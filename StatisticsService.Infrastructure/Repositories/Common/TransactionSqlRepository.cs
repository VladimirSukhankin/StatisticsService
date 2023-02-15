using System.Globalization;
using System.Text;
using AutoBogus;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StatisticsService.Domain.Entities;
using StatisticsService.Infrastructure.Dto;
using StatisticsService.Infrastructure.Dto.Filters;
using StatisticsService.Infrastructure.Repositories.Interfaces;

namespace StatisticsService.Infrastructure.Repositories.Common;

public class TransactionSqlRepository : ITransactionSqlRepository
{
    private readonly SqlPostgresDbContext _dbContext;
    private readonly IMapper _mapper;

    private const int MinCountRowsForLoad = 10;

    public TransactionSqlRepository(
        SqlPostgresDbContext dbContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<bool> AddTransactions(IEnumerable<InputTransactionDto> transactions)
    {
        long number = 0;
        try
        {
            var listTransaction = _mapper.Map<List<TransactionSql>>(transactions.ToList().DistinctBy(x=>x.TransactionNumber));
            var missingRecords = listTransaction.Where(x => !_dbContext.Transactions.Any(z => z.TransactionNumber == x.TransactionNumber)).ToList();
            if (!missingRecords.Any()) return true;
            _dbContext.AddRange(missingRecords);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            return true;
        }
        catch (Exception ex)
        {
            var s = number;
            Console.WriteLine(ex);
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

        return true;
    }

    public async Task<List<ReportTransactionPlaceDto>> GetReportTransactionPlaces()
    {
        try
        {
            return await _dbContext.Transactions
                .GroupBy(x => x.PlaceName)
                .Select(x => new ReportTransactionPlaceDto()
                {
                    PlaceName = x.Key,
                    CountTransaction = x.Count()
                })
                .OrderBy(x => x.CountTransaction)
                .ToListAsync(CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<TransactionDto> GetTransaction(int tranNo)
    {
        try
        {
            return _mapper.Map<TransactionDto>(await _dbContext.Transactions.Where(x => x.TransactionNumber == tranNo)
                .SingleOrDefaultAsync(CancellationToken.None));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IEnumerable<TransactionDto>> GetTransactions(PagingParametrs parameters)
    {
        try
        {
            return _mapper.Map<List<TransactionDto>>(
                await _dbContext.Transactions
                    .Skip(parameters.PageNumber * parameters.PageSize)
                    .Take(parameters.PageSize)
                    .ToListAsync(CancellationToken.None));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<bool> AddRandomTransactions()
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

            await AddTransactions(lstTransaction);
        }

        return true;
    }
    
}