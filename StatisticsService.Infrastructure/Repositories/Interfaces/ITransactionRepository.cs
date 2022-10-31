using Microsoft.AspNetCore.Http;
using StatisticsService.Infrastructure.Dto;

namespace StatisticsService.Infrastructure.Repositories.Interfaces;

public interface ITransactionRepository: IDisposable
{
    IEnumerable<TransactionDto> GetTransactions();
    Task<TransactionDto> GetTransaction(int tranNo);
    bool AddTransactions(IEnumerable<InputTransactionDto> transactions);
    Task<bool> AddTransactionsFromFile(IFormFile[] uploadedFiles);
}