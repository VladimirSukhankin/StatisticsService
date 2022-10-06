using ClickHouse.Ado;
using ClickHouse.Net;
using StatisticsService.Core.Settings;
using StatisticsService.Infrastructure.Repositories.Common;
using StatisticsService.Infrastructure.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddClickHouse();
builder.Services.Configure<DataBaseSettings>(
    builder.Configuration.GetSection("DataBaseSettings"));

var dataBaseSettings = builder.Configuration.GetSection("DataBaseSettings").Get<DataBaseSettings>();

builder.Services.AddClickHouse();
builder.Services.AddTransient(_ => new ClickHouseConnectionSettings(
    $"Host={dataBaseSettings.Host};Port={dataBaseSettings.Port};User={dataBaseSettings.User};" +
    $"Password={dataBaseSettings.Password};Database={dataBaseSettings.Database};Compress={dataBaseSettings.Compress};" +
    $"CheckCompressedHash={dataBaseSettings.CheckCompressedHash};SocketTimeout={dataBaseSettings.SocketTimeout};" +
    $"Compressor={dataBaseSettings.Compressor}"));

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();