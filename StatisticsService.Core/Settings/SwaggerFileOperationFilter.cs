using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StatisticsService.Core.Settings;

/// <summary>
/// Обработчик загрузки файлов
/// </summary>
public class SwaggerFileOperationFilter : IOperationFilter
{
    /// <summary>
    /// Применение фильтров
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.RelativePath == "loadTransactionsFiles" &&  operation.OperationId == "Post")
        {
            operation.Parameters = new List<OpenApiParameter>
            {
                new()
                {
                    Name = "uploadedFiles",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = "string",
                        Format = "binary",
                    },
                    In = ParameterLocation.Query,
                    Description = "Укажите путь к файлу и нажмите загрузить"
                }
            };
        }
    }
}