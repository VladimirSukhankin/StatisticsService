namespace StatisticsService.Infrastructure.Dto;

public class ReportTransactionPlaceDto
{
    public long CountTransaction { get; init; }
    
    public string? PlaceName { get; set; }
}