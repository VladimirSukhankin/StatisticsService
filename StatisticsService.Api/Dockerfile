FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StatisticsService/StatisticsService.csproj", "StatisticsService/"]
RUN dotnet restore "StatisticsService/StatisticsService.csproj"
COPY . .
WORKDIR "/src/StatisticsService"
RUN dotnet build "StatisticsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StatisticsService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StatisticsService.dll"]
