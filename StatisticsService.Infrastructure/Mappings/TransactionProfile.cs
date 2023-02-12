using System.Reflection;
using AutoMapper;
using StatisticsService.Domain.Entities;
using StatisticsService.Infrastructure.Dto;

namespace StatisticsService.Infrastructure.Mappings;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        ValueTransformers.Add<string>(val => val ?? "");

        CreateMap<TransactionDto, Transaction>()
            .ForMember(t => t.IsOnline, o => { o.MapFrom(e => (e.IsOnline != null && e.IsOnline.Value) ? "1" : "0"); })
            .ForMember(x => x.IsProlong,
                o => { o.MapFrom(e => (e.IsProlong != null && e.IsProlong.Value) ? "1" : "0"); });

        CreateMap<TransactionDto, TransactionSql>()
            .ForMember(t => t.IsOnline, o => { o.MapFrom(e => (e.IsOnline != null && e.IsOnline.Value) ? "1" : "0"); })
            .ForMember(x => x.IsProlong,
                o => { o.MapFrom(e => (e.IsProlong != null && e.IsProlong.Value) ? "1" : "0"); });


        CreateMap<Transaction, TransactionDto>()
            .ForMember(it => it.IsOnline, o =>
            {
                o.PreCondition(t => !string.IsNullOrEmpty(t.IsOnline));
                o.MapFrom(t => t.IsOnline == "1");
            })
            .ForMember(x => x.IsProlong, o =>
            {
                o.PreCondition(t => !string.IsNullOrEmpty(t.IsProlong));
                o.MapFrom(t => t.IsProlong == "1");
            })
            .ForMember(x => x.CardRefillCounter, o => { o.MapFrom(t => ConvertToNullableInt(t.CardRefillCounter)); })
            .ForMember(x => x.TicketRemainingTripsCounter,
                o => { o.MapFrom(t => ConvertToNullableInt(t.TicketRemainingTripsCounter)); })
            .ForMember(x => x.CardUsageCounter, o => { o.MapFrom(t => ConvertToNullableInt(t.CardUsageCounter)); });
        ;

        CreateMap<TransactionSql, TransactionDto>()
            .ForMember(it => it.IsOnline, o =>
            {
                o.PreCondition(t => !string.IsNullOrEmpty(t.IsOnline));
                o.MapFrom(t => t.IsOnline == "1");
            })
            .ForMember(x => x.IsProlong, o =>
            {
                o.PreCondition(t => !string.IsNullOrEmpty(t.IsProlong));
                o.MapFrom(t => t.IsProlong == "1");
            })
            .ForMember(x => x.CardRefillCounter, o => { o.MapFrom(t => ConvertToNullableInt(t.CardRefillCounter)); })
            .ForMember(x => x.TicketRemainingTripsCounter, 
                o =>
                {
                    o.MapFrom(t => ConvertToNullableInt(t.TicketRemainingTripsCounter)); 
                    
                })
            .ForMember(x => x.CardUsageCounter, o =>
            {
                o.MapFrom(t => ConvertToNullableInt(t.CardUsageCounter)); 
                
            });

        CreateMap<InputTransactionDto, Transaction>().ReverseMap();
        CreateMap<InputTransactionDto, TransactionSql>().ReverseMap();
    }

    private static int? ConvertToNullableInt(string? value) => int.TryParse(value, out var i) ? i : null;
}