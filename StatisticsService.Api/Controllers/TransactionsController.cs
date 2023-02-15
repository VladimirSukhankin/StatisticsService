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
    private readonly ITransactionSqlRepository _transactionSqlRepository;

    /// <summary>
    /// Конструктор
    /// </summary>
    public TransactionsController(ITransactionRepository transactionRepository, ITransactionSqlRepository transactionSqlRepository)
    {
        _transactionRepository = transactionRepository;
        _transactionSqlRepository = transactionSqlRepository;
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
    /// Добавление рандомных транзакций
    /// </summary>
    [HttpPost("addRandomTransactions")]
    public async Task<ActionResult> AddRandom()
    {
        return await _transactionRepository.AddRandomTransactions() ? Ok() : StatusCode(400);
    }
    
    /// <summary>
    /// Добавление рандомных транзакций в Postgres
    /// </summary>
    [HttpPost("addRandomTransactionsSql")]
    public async Task<ActionResult> AddRandomSql()
    {
        return await _transactionSqlRepository.AddRandomTransactions() ? Ok() : StatusCode(400);
    }
    
    /// <summary>
    /// Добавление транзакций
    /// </summary>
    [HttpPost("addTransactions")]
    public async Task<ActionResult> AddTransactions([FromBody] IEnumerable<InputTransactionDto> transactions)
    {
        return await _transactionRepository.AddTransactions(transactions) ? Ok() : StatusCode(400);
    }
    
    /// <summary>
    /// Добавление транзакций в PostgreSql
    /// </summary>
    [HttpPost("addTransactionsSql")]
    public async Task<ActionResult> AddTransactionsSql([FromBody] IEnumerable<InputTransactionDto> transactions)
    {
        return await _transactionSqlRepository.AddTransactions(transactions) ? Ok() : StatusCode(400);
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
    /// Добавление транзакций через файл в PostgreSql
    /// </summary>
    [HttpPost]
    [DisableRequestSizeLimit,
     RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
         ValueLengthLimit = int.MaxValue)]
    [Route("loadTransactionsFilesSql")]
    public async Task<ActionResult> PostSql(IFormFile[] uploadedFiles)
    {
        return await _transactionSqlRepository.AddTransactionsFromFile(uploadedFiles) ? Ok() : BadRequest();
    }
    
    /// <summary>
    /// Получение транзакции по номеру
    /// </summary>
    [HttpGet("getTransaction")]
    public async Task<ActionResult<TransactionDto>> GetTransaction([FromQuery] int transactionNumber)
    {
        return await _transactionRepository.GetTransaction(transactionNumber);;
    }
    
    /// <summary>
    /// Получение транзакции по номеру Sql
    /// </summary>
    [HttpGet("getTransactionSql")]
    public async Task<ActionResult<TransactionDto>> GetTransactionSql([FromQuery] int transactionNumber)
    {
        return await _transactionSqlRepository.GetTransaction(transactionNumber);;
    }
    
    /// <summary>
    /// Получение всех транзакций c пагинацией
    /// </summary>
    [HttpGet("getTransactions")]
    public async Task<List<TransactionDto>> GetTransactions([FromQuery] PagingParametrs parameters)
    {
        var s = await _transactionRepository.GetTransactions(parameters);
        return s.ToList();
    }

    /// <summary>
    /// Получение всех транзакций c пагинацией
    /// </summary>
    [HttpGet("getTransactionsSql")]
    public async Task<IEnumerable<TransactionDto>> GetTransactionsSql([FromQuery] PagingParametrs parameters)
    {
        return await _transactionSqlRepository.GetTransactions(parameters);;
    }
    
    /// <summary>
    /// Получение отчёта по месту прохода
    /// </summary>
    [HttpGet]
    [Route("getReportTransactionsPlace")]
    public Task<List<ReportTransactionPlaceDto>> GetReportTransactionPlace()
    {
        return _transactionRepository.GetReportTransactionPlaces();
    }

    /// <summary>
    /// Получение отчёта по месту прохода
    /// </summary>
    [HttpGet]
    [Route("getReportTransactionsPlaceSql")]
    public Task<List<ReportTransactionPlaceDto>> GetReportTransactionPlaceSql()
    {
        return _transactionSqlRepository.GetReportTransactionPlaces();
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