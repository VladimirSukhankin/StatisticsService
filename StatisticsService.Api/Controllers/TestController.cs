using Microsoft.AspNetCore.Mvc;
using StatisticsService.Infrastructure;
using StatisticsService.Infrastructure.Dto;
using StatisticsService.Infrastructure.Repositories.Interfaces;

namespace StatisticsService.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ITransactionRepository transactionRepository;

    public TestController(ITransactionRepository transactionRepository)
    {
        this.transactionRepository = transactionRepository;
    }

    [HttpOptions]
    public async Task<IEnumerable<TransactionDto>> Test()
    {
        return await transactionRepository.GetTransactions();
    }
}