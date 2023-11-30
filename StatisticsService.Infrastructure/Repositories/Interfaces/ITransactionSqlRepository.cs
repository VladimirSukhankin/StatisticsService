using Microsoft.AspNetCore.Http;
using StatisticsService.Infrastructure.Dto;
using StatisticsService.Infrastructure.Dto.Filters;

namespace StatisticsService.Infrastructure.Repositories.Interfaces;

public interface ITransactionSqlRepository
{
    Task<bool> AddTransactions(IEnumerable<InputTransactionDto> transactions);
    
    Task<bool> AddTransactionsFromFile(IEnumerable<IFormFile> uploadedFiles);
    
    Task<List<ReportTransactionPlaceDto>> GetReportTransactionPlaces();
    
    Task<TransactionDto> GetTransaction(int tranNo);
    
    Task<IEnumerable<TransactionDto>> GetTransactions(PagingParametrs parameters);

    Task<bool> AddRandomTransactions();
}