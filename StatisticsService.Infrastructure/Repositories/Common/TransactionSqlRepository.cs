using System.Text;
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

    public async Task<bool> AddTransactionsFromFile(IFormFile[] uploadedFiles)
    {
        foreach (var file in uploadedFiles)
        {
            var instance = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(instance);

            var inputTransactions = new List<InputTransactionDto>();
            var countRows = 0;

            using (var streamReader = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding(1251)))
            using (var reader = new JsonTextReader(streamReader))
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

    public IEnumerable<ReportTransactionPlaceDto> GetReportTransactionPlaces()
    {
        try
        {
            return _dbContext.Transactions.GroupBy(x => x.PlaceName).Select(x => new ReportTransactionPlaceDto()
                {
                    PlaceName = x.Key,
                    CountTransaction = x.Count()
                })
                .OrderBy(x => x.CountTransaction)
                .AsEnumerable();
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
}