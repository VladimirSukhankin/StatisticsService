# StatisticsService
Service for collecting statistics on passes and ticket sales

# Инструкция по работе из контейнера
1) загрузить образ сервера click-house docker pull clickhouse/clickhouse-server
2) docker run -e CLICKHOUSE_DB=statistics -e CLICKHOUSE_USER=username -e CLICKHOUSE_DEFAULT_ACCESS_MANAGEMENT=1 -e CLICKHOUSE_PASSWORD=password -p 18123:8123 -p 18999:9000 --name test_new clickhouse/clickhouse-server
3) Рекомендуется скачать https://dbeaver.io/download/ чтобы создать таблицу. Необходимо подключится к контейнеру к хосту localhost к порту 18123, с имененм username и паролем password. и выполнить скрипт
create table statistics.transactions
(
    TransactionNumber Int64,
    TransactionTypeName Nullable(String),
    ApplicationName Nullable(String),
    PlaceName Nullable(String),
    TransactionDate Nullable(String),
    DeviceSerialNumber Nullable(String),
    AgentName Nullable(String),
    TicketGuid Nullable(String),
    CardNumber Nullable(Int64),
    CardSerialNumber Nullable(Int64),
    ProductName Nullable(String),
    IsOnline Nullable(String),
    TicketDateBegin Nullable(String),
    TicketRemainingTripsCounter Nullable(String),
    CardRefillCounter Nullable(String),
    CardBalance Nullable(String),
    TicketRegisterDate Nullable(String),
    CardUsageCounter Nullable(String),
    IsProlong Nullable(String),
    Price Nullable(String),
    TicketDateEnd Nullable(String)
)
    engine = MergeTree PRIMARY KEY TransactionNumber
        ORDER BY TransactionNumber
        SETTINGS index_granularity = 8192;

		
4) скачать имейдж сервиса отчётов docker pull revsuhlab/statistics-services:version2
5) выполнить для него docker run --name statistics-service -p 8080:80 -p 8081:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8081 -e ASPNETCORE_ENVIRONMENT=Development revsuhlab/statistics-services:version1
6) проверить на https://localhost:8081 Options запрос
7)docker network create statistics-network
docker network inspect statistics-network
docker network connect statistics-network test_new
docker network connect statistics-network statistics-service
8) можно загрузить транзакции через файл с https://disk.yandex.ru/d/ULtgjg1JX--J9g  или в ручную

# Для запуска на локальной машине:
1) необходимо воспроизвести пункты с 1-3 из инструкции работы с контейнерами.
2) создать папку в userSecrets с названием 1F8A3E4B-1C22-43A7-8EBC-3D69F416BCBD для работы с https и положить туда https://disk.yandex.ru/d/Am-pd8icWZxdZw
3) Плюс создать локальный сертификат или скачать с https://disk.yandex.ru/d/Rb7yqT3oUPVGIw 
