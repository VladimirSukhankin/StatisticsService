namespace StatisticsService.Infrastructure.Dto.Filters;

/// <summary>
/// Объект для постраничного вывода.
/// </summary>
public class PagingParametrs
{
    /// <summary>
    /// Номер страницы.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Размер страницы.
    /// </summary>
    public int PageSize { get; set; } = 100;
}