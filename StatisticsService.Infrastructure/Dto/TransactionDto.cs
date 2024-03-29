﻿namespace StatisticsService.Infrastructure.Dto;

/// <summary>
/// Объект для передачи.
/// </summary>
public class TransactionDto
{
    /// <summary>
    /// Номер транзакции на устройстве.
    /// </summary>
    public long TransactionNumber { get; set; }

    /// <summary>
    /// Наименование типа транзакции.
    /// </summary>
    public string? TransactionTypeName { get; set; }

    /// <summary>
    /// Наименование приложения.
    /// </summary>
    public string? ApplicationName { get; set; }
    
    /// <summary>
    /// Наименование места прохода.
    /// </summary>
    public string? PlaceName { get; set; }
    
    /// <summary>
    /// Дата и время транзакции.
    /// </summary>
    public string? TransactionDate { get; set; }

    /// <summary>
    /// Серийный номер устройства.
    /// </summary>
    public string? DeviceSerialNumber { get; set; }

    /// <summary>
    /// Идентификатор контрагента.
    /// </summary>
    public string? AgentName { get; set; }

    /// <summary>
    /// Уникальный номер билета.
    /// </summary>
    public string? TicketGuid { get; set; }

    /// <summary>
    /// Транспортный номер карты.
    /// </summary>
    public long? CardNumber { get; set; }

    /// <summary>
    /// Номер кристалла БСК.
    /// </summary>
    public long? CardSerialNumber { get; set; }

    /// <summary>
    /// Наименование продукта
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// Признак оформления билета в ЦОД (0 - нет, 1 - да).
    /// </summary>
    public bool? IsOnline { get; set; }

    /// <summary>
    /// Дата начала действия билета.
    /// </summary>
    public string? TicketDateBegin { get; set; }

    /// <summary>
    /// Дата окончания действия.
    /// </summary>
    public string? TicketDateEnd { get; set; }

    /// <summary>
    /// Количество оставшихся проходов.
    /// </summary>
    public int? TicketRemainingTripsCounter { get; set; } = 0;

    /// <summary>
    /// Количество пополнений карты.
    /// </summary>
    public int? CardRefillCounter { get; set; } = 0;

    /// <summary>
    /// Сумма остатка на карте.
    /// </summary>
    public string? CardBalance { get; set; }

    /// <summary>
    /// Дата и время оформления билета.
    /// </summary>
    public string? TicketRegisterDate { get; set; }

    /// <summary>
    /// Количество совершенных проходов.
    /// </summary>
    public int? CardUsageCounter { get; set; } = 0;

    /// <summary>
    /// Флаг продления (true - да, false - нет).
    /// </summary>
    public bool? IsProlong { get; set; }

    /// <summary>
    /// Стоимость прохода.
    /// </summary>
    public string? Price { get; set; }
}