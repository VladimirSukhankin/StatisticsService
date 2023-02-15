using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace VersusDatabaseTest;

    public class BasicTests
        : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public BasicTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetTransactionsFromClickHouse()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Transactions/getTransactions");
        }

        [Fact]
        public async Task GetTransactionsFromPostgres()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Transactions/getTransactionsSql");
        }
        
        [Fact]
        public async Task GetReportPlaceFromClickHouse()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Transactions/getReportTransactionsPlace");
        }

        [Fact]
        public async Task GetReportPlaceFromPostgres()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Transactions/getReportTransactionsPlaceSql");
        }
        
    }
