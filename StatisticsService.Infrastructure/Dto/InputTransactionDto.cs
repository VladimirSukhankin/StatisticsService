using Newtonsoft.Json;

namespace StatisticsService.Infrastructure.Dto;

public class InputTransactionDto
{
    /// <summary>
    /// Номер транзакции на устройстве.
    /// </summary>
    [JsonProperty("tranNo")]
    public long TransactionNumber { get; set; }

    /// <summary>
    /// Наименование типа транзакции.
    /// </summary>
    [JsonProperty("tranTypeNameShort")]
    public string? TransactionTypeName { get; set; }

    /// <summary>
    /// Наименование приложения.
    /// </summary>
    [JsonProperty("appName")]
    public string? ApplicationName { get; set; }
    
    /// <summary>
    /// Наименование места прохода.
    /// </summary>
    [JsonProperty("placeName")]
    public string? PlaceName { get; set; }
    
    /// <summary>
    /// Дата и время транзакции.
    /// </summary>
    [JsonProperty("tranDate")]
    public string? TransactionDate { get; set; }

    /// <summary>
    /// Серийный номер устройства.
    /// </summary>
    [JsonProperty("deviceNo")]
    public string? DeviceSerialNumber { get; set; }

    /// <summary>
    /// Идентификатор контрагента.
    /// </summary>
    [JsonProperty("agentNameShort")]
    public string? AgentName { get; set; }

    /// <summary>
    /// Уникальный номер билета.
    /// </summary>
    [JsonProperty("ticketUid")]
    public string? TicketGuid { get; set; }

    /// <summary>
    /// Транспортный номер карты.
    /// </summary>
    [JsonProperty("crdNo")]
    public string? CardNumber { get; set; }

    /// <summary>
    /// Номер кристалла БСК.
    /// </summary>
    [JsonProperty("crdSerialNo")]
    public string? CardSerialNumber { get; set; }

    /// <summary>
    /// Наименование продукта
    /// </summary>
    [JsonProperty("gdNameShort")]
    public string? ProductName { get; set; }

    /// <summary>
    /// Признак оформления билета в ЦОД (0 - нет, 1 - да).
    /// </summary>
    [JsonProperty("isOnline")]
    public string? IsOnline { get; set; }

    /// <summary>
    /// Дата начала действия билета.
    /// </summary>
    [JsonProperty("crdDateBegin")]
    public string? TicketDateBegin { get; set; }

    /// <summary>
    /// Дата окончания действия.
    /// </summary>
    [JsonProperty("crdDateEnd")]
    public string? TicketDateEnd { get; set; }

    /// <summary>
    /// Количество оставшихся проходов.
    /// </summary>
    [JsonProperty("crdRemainingTripsCounter")]
    public string? TicketRemainingTripsCounter { get; set; }

    /// <summary>
    /// Количество пополнений карты.
    /// </summary>
    [JsonProperty("crdRefillCounter")]
    public string? CardRefillCounter { get; set; }

    /// <summary>
    /// Сумма остатка на карте.
    /// </summary>
    [JsonProperty("crdBalance")]
    public string? CardBalance { get; set; }

    /// <summary>
    /// Дата и время оформления билета.
    /// </summary>
    [JsonProperty("ticketRegisterDate")]
    public string? TicketRegisterDate { get; set; }

    /// <summary>
    /// Количество совершенных проходов.
    /// </summary>
    [JsonProperty("crdUsageCounter")]
    public int? CardUsageCounter { get; set; }

    /// <summary>
    /// Флаг продления (true - да, false - нет).
    /// </summary>
    [JsonProperty("isProlong")]
    public string? IsProlong { get; set; }

    /// <summary>
    /// Стоимость прохода.
    /// </summary>
    [JsonProperty("price")]
    public string? Price { get; set; }
}