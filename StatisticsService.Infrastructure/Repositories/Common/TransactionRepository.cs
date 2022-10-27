using System.Collections;
using System.ComponentModel;
using AutoMapper;
using ClickHouse.Ado;
using ClickHouse.Net;
using StatisticsService.Domain.Entities;
using StatisticsService.Infrastructure.Dto;
using StatisticsService.Infrastructure.Repositories.Interfaces;

namespace StatisticsService.Infrastructure.Repositories.Common;

public class TransactionRepository : ITransactionRepository
{
    private readonly IClickHouseDatabase _database;
    private readonly IClickHouseConnectionFactory _connection;
    private readonly IMapper _mapper;

    public TransactionRepository(IClickHouseDatabase database, IMapper mapper, IClickHouseConnectionFactory connection)
    {
        _database = database;
        _mapper = mapper;
        _connection = connection;
    }

    public void Dispose()
    {
        try
        {
            _database.Open(); 
            _database.Close();
        }
        catch
        {
            _database.Close();
        }
    }

    public IEnumerable<TransactionDto> GetTransactions()
    {
        try
        {
            _database.Open();
            var ss = _database.ExecuteQueryMapping<Transaction>("select * from transactions").ToList();
            return _mapper.Map<List<TransactionDto>>(ss);
        }
        catch (Exception ex)
        {
            return null!;
        }
    }

    
    
    
    public Task<TransactionDto> GetTransaction(int tranNo)
    {
        throw new NotImplementedException();
    }

    public bool AddTransactions(IEnumerable<InputTransactionDto> transactions)
    {
        try
        {
            _database.Open();
            var tt = _mapper.Map<List<Transaction>>(transactions);
            _database.BulkInsert("transactions", new Transaction().GetType().GetProperties().Select(x => x.Name).ToList(), _mapper.Map<List<Transaction>>(transactions));
           
           
            return  true;
        }
        catch (Exception ee)
        {
            return false;
        }
    }
}

