using StatisticsService.Domain.Entities;
using StatisticsService.Infrastructure.Dto;

namespace StatisticsService.Infrastructure.Repositories.Interfaces;

public interface ITransactionRepository: IDisposable
{
    Task<IEnumerable<TransactionDto>> GetTransactions();
    Task<TransactionDto> GetTransaction(int tranNo);
    Task<TransactionDto> AddTransactions(IEnumerable<TransactionDto> transactions);
}