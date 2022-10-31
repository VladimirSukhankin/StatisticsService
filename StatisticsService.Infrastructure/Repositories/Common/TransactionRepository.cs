using System.Text;
using AutoMapper;
using ClickHouse.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using StatisticsService.Domain.Entities;
using StatisticsService.Infrastructure.Dto;
using StatisticsService.Infrastructure.Repositories.Interfaces;

namespace StatisticsService.Infrastructure.Repositories.Common;

public class TransactionRepository : ITransactionRepository
{
    private readonly IClickHouseDatabase _database;
    private readonly IMapper _mapper;
    private const int MIN_COUNT_ROWS_FOR_LOAD = 100000;
    public TransactionRepository(IClickHouseDatabase database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
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
            _database.BulkInsert("transactions",
                new Transaction().GetType().GetProperties().Select(x => x.Name).ToList(),
                _mapper.Map<List<Transaction>>(transactions));

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AddTransactionsFromFile(IFormFile[] uploadedFiles)
    {
        foreach (var file in uploadedFiles)
        {
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var instance = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(instance);
            var jsonString = Encoding.GetEncoding(1251).GetString(memoryStream.ToArray());
            var array = JArray.Parse(jsonString);
            var countRows = 0;
            var inputTransactions = new List<InputTransactionDto>();
            foreach (var obj in array.Children<JObject>())
            {
                try
                {
                    inputTransactions.Add(obj.ToObject<InputTransactionDto>() ?? throw new InvalidOperationException());
                    countRows++;
                    if (countRows == MIN_COUNT_ROWS_FOR_LOAD)
                    {
                        AddTransactions(inputTransactions);
                        inputTransactions.Clear();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            if (inputTransactions.Count <= 0) continue;
            AddTransactions(inputTransactions);
        }

        return await Task.Run(() => true);
    }
}