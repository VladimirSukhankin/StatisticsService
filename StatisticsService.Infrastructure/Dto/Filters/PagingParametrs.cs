namespace StatisticsService.Infrastructure.Dto.Filters;

/// <summary>
/// Объект для постраничного вывода.
/// </summary>
public class PagingParametrs
{
    /// <summary>
    /// Номер страницы.
    /// </summary>
    public int PageNumber => 4;

    /// <summary>
    /// Размер страницы.
    /// </summary>
    public int PageSize => 100000;
}