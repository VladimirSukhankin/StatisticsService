using Microsoft.AspNetCore.Http;
using StatisticsService.Infrastructure.Dto;
using StatisticsService.Infrastructure.Dto.Filters;

namespace StatisticsService.Infrastructure.Repositories.Interfaces;

public interface ITransactionRepository : IDisposable
{
    IEnumerable<TransactionDto> GetTransactions(PagingParametrs parametrs);
    Task<TransactionDto> GetTransaction(int tranNo);
    bool AddTransactions(IEnumerable<InputTransactionDto> transactions);
    Task<bool> AddTransactionsFromFile(IFormFile[] uploadedFiles);
    IEnumerable<ReportTransactionPlaceDto> GetReportTransactionPlaces();
    IEnumerable<TransactionDto> GetCountTransactionsForRangeDate(DataRangeFilter dataRangeFilter, PagingParametrs parametrs);
}