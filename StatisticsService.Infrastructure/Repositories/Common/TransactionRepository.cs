using ClickHouse.Net;
using StatisticsService.Domain.Entities;
using StatisticsService.Infrastructure.Dto;
using StatisticsService.Infrastructure.Repositories.Interfaces;

namespace StatisticsService.Infrastructure.Repositories.Common;

public class TransactionRepository: ITransactionRepository
{
    private readonly IClickHouseDatabase database;

    public TransactionRepository(IClickHouseDatabase database)
    {
        this.database = database;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransactionDto>> GetTransactions()
    {
        try
        {
            database.Open();
            var array = database.ExecuteQueryMapping<Transaction>("select * from transactions");
        }
        catch (Exception ex)
        {
            ;
        }

        return null;
    }

    public Task<TransactionDto> GetTransaction(int tranNo)
    {
        throw new NotImplementedException();
    }

    public Task<TransactionDto> AddTransactions(IEnumerable<TransactionDto> transactions)
    {
        throw new NotImplementedException();
    }
}