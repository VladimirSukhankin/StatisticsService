using System.Data.Common;
using ClickHouse.Ado;
using ClickHouse.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using StatisticsService.Infrastructure;
using StatisticsService.Infrastructure.Mappings;
using StatisticsService.Infrastructure.Repositories.Common;
using StatisticsService.Infrastructure.Repositories.Interfaces;

namespace VersusDatabaseTest;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<SqlPostgresDbContext>));

            services.Remove(dbContextDescriptor!);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbConnection));

            services.Remove(dbConnectionDescriptor!);

            services.AddSingleton<DbConnection>(_ =>
            {
                var connection = new NpgsqlConnection("Server=localhost;Port=5432;User ID=postgres;Database=statistics;Password=1;TrustServerCertificate=True");
                connection.Open();

                return connection;
            });

            services.AddDbContext<SqlPostgresDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseNpgsql(connection);
            });
            
            services.AddClickHouse();

            services.AddTransient(_ => new ClickHouseConnectionSettings(
                "Host=localhost;Port=18999;User=username;" +
                "Password=password;Database=statistics;Compress=True;" +
                "CheckCompressedHash=False;SocketTimeout=60000000;" +
                "Compressor=lz4"));
            
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionSqlRepository, TransactionSqlRepository>();

           services.AddAutoMapper(typeof(TransactionProfile));

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });
        });

        builder.UseEnvironment("Development");
        

    }
}