using System.Reflection;
using ClickHouse.Ado;
using ClickHouse.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StatisticsService.Core.Settings;
using StatisticsService.Infrastructure;
using StatisticsService.Infrastructure.Mappings;
using StatisticsService.Infrastructure.Repositories.Common;
using StatisticsService.Infrastructure.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.SwaggerDoc("ApiSpecification", new OpenApiInfo()
    {
        Title = "API",
        Version = "1"
    });
    setupAction.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    setupAction.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});
builder.Services.AddSwaggerGenNewtonsoftSupport();

var dataBaseSettings = builder.Configuration.GetSection("DataBaseSettings").Get<DataBaseSettings>();

builder.Services.AddClickHouse();

builder.Services.AddTransient(_ => new ClickHouseConnectionSettings(
    $"Host={dataBaseSettings.Host};Port={dataBaseSettings.Port};User={dataBaseSettings.User};" +
    $"Password={dataBaseSettings.Password};Database={dataBaseSettings.Database};Compress={dataBaseSettings.Compress};" +
    $"CheckCompressedHash={dataBaseSettings.CheckCompressedHash};SocketTimeout={dataBaseSettings.SocketTimeout};" +
    $"Compressor={dataBaseSettings.Compressor}"));

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionSqlRepository, TransactionSqlRepository>();

builder.Services.AddAutoMapper(typeof(TransactionProfile));

builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
});

    builder.Services.AddDbContext<SqlPostgresDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("postgresConnection")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(setupAction =>
{
    setupAction.SwaggerEndpoint(
        "./swagger/ApiSpecification/swagger.json",
        "API");
    setupAction.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();