using Microsoft.AspNetCore.Mvc;
using StatisticsService.Infrastructure.Dto;
using StatisticsService.Infrastructure.Repositories.Interfaces;

namespace StatisticsService.Controllers;



/// <summary>
/// Контроллер для управления транзакциями
/// </summary>
[ApiController]
[Route("[controller]")]
public class TransactionsController : ControllerBase
{
    readonly ITransactionRepository _transactionRepository;
    /// <summary>
    /// Конструктор
    /// </summary>
    public TransactionsController(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    /// <summary>
    /// Проверка работоспособности API
    /// </summary>
    [HttpOptions]
    public ActionResult Test()
    {
        return Ok();
    }

    /// <summary>
    /// Добавление транзакций
    /// </summary>
    [HttpPost("addTransactions")]
    public ActionResult AddTransactions([FromBody] IEnumerable<InputTransactionDto> transactions)
    {
        return _transactionRepository.AddTransactions(transactions) ? Ok() : StatusCode(400);
    }
    
    /// <summary>
    /// Получение всех транзакций
    /// </summary>
    [HttpGet("getTransactions")]
    public IEnumerable<TransactionDto> GetTransactions()
    {
        return _transactionRepository.GetTransactions();
    }

    /// <summary>
    /// Добавление транзакций через файл
    /// </summary>
    [HttpPost]
    [DisableRequestSizeLimit,
     RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, 
         ValueLengthLimit = int.MaxValue)]
    [Route("loadTransactionsFiles")]
    public async Task<ActionResult<IFormFile>> Post(IFormFile[] uploadedFiles)
    {
        await _transactionRepository.AddTransactionsFromFile(uploadedFiles);
        return null;
    }
}