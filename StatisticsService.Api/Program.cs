using System.Reflection;
using ClickHouse.Ado;
using ClickHouse.Net;
using Microsoft.OpenApi.Models;
using StatisticsService.Core.Settings;
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
    setupAction.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
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

builder.Services.AddAutoMapper(typeof(TransactionProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(setupAction =>
    {
        setupAction.SwaggerEndpoint(
            "./swagger/ApiSpecification/swagger.json",
            "API");
        setupAction.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

