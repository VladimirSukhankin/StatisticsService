using StatisticsService.Infrastructure.Dto;
using StatisticsService.Infrastructure.Dto.Filters;

namespace StatisticsService.Infrastructure.Repositories.Interfaces;

public interface ITransactionRepository : ITransactionSqlRepository {
    
    IEnumerable<TransactionDto> GetReportTransactionsForRangeDate(DataRangeFilter dataRangeFilter, PagingParametrs parameters);

    IEnumerable<TransactionDto> GetReportTransactionsWithNotProlong(PagingParametrs parameters);
}