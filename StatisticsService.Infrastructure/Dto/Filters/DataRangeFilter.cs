namespace StatisticsService.Infrastructure.Dto.Filters;

/// <summary>
/// Объект фильтра с диапазоном дат.
/// </summary>
public class DataRangeFilter
{
    /// <summary>
    /// Начала диапазона дат.
    /// </summary>
    public DateTime StartRange { get; set; }
    
    /// <summary>
    /// Конец диапазона дат.
    /// </summary>
    public DateTime EndRange { get; set; }
}