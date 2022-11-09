using Microsoft.AspNetCore.Mvc;
using StatisticsService.Infrastructure.Dto;
using StatisticsService.Infrastructure.Dto.Filters;
using StatisticsService.Infrastructure.Repositories.Interfaces;

namespace StatisticsService.Controllers;

/// <summary>
/// Контроллер для управления транзакциями
/// </summary>
[ApiController]
[Route("[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionRepository _transactionRepository;

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
    /// Добавление транзакций через файл
    /// </summary>
    [HttpPost]
    [DisableRequestSizeLimit,
     RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
         ValueLengthLimit = int.MaxValue)]
    [Route("loadTransactionsFiles")]
    public async Task<ActionResult> Post(IFormFile[] uploadedFiles)
    {
        return await _transactionRepository.AddTransactionsFromFile(uploadedFiles) ? Ok() : BadRequest();
    }

    /// <summary>
    /// Получение всех транзакций c пагинацией
    /// </summary>
    [HttpGet("getTransactions")]
    public IEnumerable<TransactionDto> GetTransactions([FromQuery] PagingParametrs parametrs)
    {
        return _transactionRepository.GetTransactions(parametrs);;
    }

    
    /// <summary>
    /// Получение отчёта по месту прохода
    /// </summary>
    [HttpGet]
    [Route("getReportTransactionsPlace")]
    public IEnumerable<object> GetReportTransactionPlace()
    {
        return _transactionRepository.GetReportTransactionPlaces();
    }

    /// <summary>
    /// Получение отчёта по дате прохода с пагинацией
    /// </summary>
    [HttpGet]
    [Route("getReportTransactionsDataRange")]
    public IEnumerable<TransactionDto> GetReportTransactionsDataRange([FromQuery] DataRangeFilter filter, [FromQuery]PagingParametrs parametrs)
    {
        return _transactionRepository.GetReportTransactionsForRangeDate(filter, parametrs);
    }
    
    /// <summary>
    /// Получение отчёта по непродленным билетам
    /// </summary>
    [HttpGet]
    [Route("getReportTransactionsWithNotProlong")]
    public IEnumerable<TransactionDto> GetReportTransactionsNotProlong([FromQuery]PagingParametrs parametrs)
    {
        return _transactionRepository.GetReportTransactionsWithNotProlong(parametrs);
    }
}