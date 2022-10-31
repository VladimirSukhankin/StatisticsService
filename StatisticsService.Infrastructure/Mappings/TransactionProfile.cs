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
            .ForMember(t => t.IsOnline, o => { o.MapFrom(e => (e.IsOnline != null && e.IsOnline.Value) ? "Y" : "N"); })
            .ForMember(x => x.IsProlong,
                o => { o.MapFrom(e => (e.IsProlong != null && e.IsProlong.Value) ? "Y" : "N"); })
            .ReverseMap();

        CreateMap<Transaction, TransactionDto>()
            .ForMember(it => it.IsOnline, o =>
            {
                o.PreCondition(t => !string.IsNullOrEmpty(t.IsOnline));
                o.MapFrom(t => t.IsOnline == "Y");
            })
            .ForMember(x => x.IsProlong, o =>
            {
                o.PreCondition(t => !string.IsNullOrEmpty(t.IsProlong));
                o.MapFrom(t => t.IsProlong == "Y");
            })
            .ForMember(t => t.CardBalance, o =>
            {
                o.PreCondition(e => !string.IsNullOrEmpty(e.CardBalance));
                o.MapFrom(t => Decimal.Parse(t.CardBalance));
            })
            .ForMember(t => t.Price, o =>
            {
                o.PreCondition(e => !string.IsNullOrEmpty(e.Price));
                o.MapFrom(t => Decimal.Parse(t.Price));
            });

        CreateMap<InputTransactionDto, Transaction>().ReverseMap();
    }
}