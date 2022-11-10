FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StatisticsService.Api/StatisticsService.Api.csproj", "StatisticsService.Api/"]
COPY ["StatisticsService.Core/StatisticsService.Core.csproj", "StatisticsService.Core/"]
COPY ["StatisticsService.Infrastructure/StatisticsService.Infrastructure.csproj", "StatisticsService.Infrastructure/"]
COPY ["StatisticsService.Domain/StatisticsService.Domain.csproj", "StatisticsService.Domain/"]
RUN dotnet restore "StatisticsService.Api/StatisticsService.Api.csproj"
COPY . .
WORKDIR "/src/StatisticsService.Api"
RUN dotnet build "StatisticsService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StatisticsService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StatisticsService.Api.dll"]
