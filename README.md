# OzonSync / OzonAnalytics

Бэкенд-сервис на ASP.NET Core, который синхронизирует заказы продавца из Ozon Seller API в PostgreSQL и отдаёт аналитику продаж через REST API. Многопользовательский: каждый пользователь видит только свои магазины.

> Проект учебный и портфолио-ориентированный; он также служит основой магистерской работы по теме автоматизированного сбора и анализа данных о продажах на маркетплейсе Ozon.

## Возможности

- **Аутентификация:** ASP.NET Core Identity, JWT access-токены и refresh-токены.
- **Магазины:** подключение магазина по Client-Id и API-ключу Ozon; ключ проверяется при подключении и хранится в базе в зашифрованном виде.
- **Синхронизация заказов:** постраничная (курсорная), идемпотентная (повторный запуск не создаёт дублей и обновляет изменившиеся статусы), с журналом запусков (`SyncLogs`) и статусами магазина.
- **Защита от параллельных запусков:** блокировка синхронизации магазина в Redis (`SET NX` с TTL, освобождение Lua-скриптом по токену владельца).
- **Нормализация статусов:** статусы и подстатусы отправлений Ozon приводятся к ограниченному набору состояний (`Assembling`, `HandedOver`, `InTransit`, `Delivered`, `Problem`, `Cancelled`, `Unknown`).
- **Аналитика:** сводка, сравнение с предыдущим периодом, топ товаров, динамика по дням.
- **Кэш аналитики в Redis:** cache-aside с версией кэша магазина; после синхронизации версия растёт, и устаревшие ответы перестают использоваться. При недоступном Redis ответы считаются из базы.
- **Режим без Ozon:** `FakeOzonClient` для разработки и тестов без реального ключа.

## Архитектура

Слоистый монолит. Слой `Application` не знает про `Infrastructure`: интерфейсы и DTO лежат в `Application`, реализации в `Infrastructure`.

```
OzonAnalytics.Api            контроллеры, middleware обработки ошибок, настройка приложения
OzonAnalytics.Application    сервисы, интерфейсы, DTO, маппинг статусов, исключения
OzonAnalytics.Domain         сущности и перечисления
OzonAnalytics.Infrastructure EF Core, репозитории, клиент Ozon, Redis, шифрование
OzonAnalytics.Worker         фоновая синхронизация (в разработке)
OzonAnalytics.Tests          unit-тесты
```

## Стек

ASP.NET Core · Entity Framework Core · PostgreSQL · Redis (StackExchange.Redis) · ASP.NET Core Identity + JWT · xUnit + Moq

## API (основное)

Все методы, кроме регистрации и входа, требуют JWT (`Authorization: Bearer ...`).

| Метод | Путь | Описание |
|---|---|---|
| POST | `/api/v1/auth/Registration` | регистрация пользователя |
| POST | `/api/v1/auth/Login` | вход, выдача access- и refresh-токенов |
| POST | `/api/v1/shops/connect` | подключить магазин (проверка ключа Ozon) |
| GET | `/api/v1/shops` | список магазинов пользователя |
| GET | `/api/v1/shops/{shopId}` | магазин по идентификатору |
| POST | `/api/v1/shops/{shopId}/sync` | запустить синхронизацию (409, если уже идёт) |
| GET | `/api/v1/shops/{shopId}/logs` | журнал синхронизаций |
| GET | `/api/v1/shops/{shopId}/analytics/summary` | сводка за период |
| GET | `/api/v1/shops/{shopId}/analytics/compare` | сравнение с предыдущим периодом |
| GET | `/api/v1/shops/{shopId}/analytics/top-product` | топ товаров (`limit`, `sortBy`) |
| GET | `/api/v1/shops/{shopId}/analytics/time-series` | динамика по дням |

Полный список методов виден в Swagger (`/swagger`).

### Правила расчёта аналитики

- Продажа это заказ в состоянии `Delivered`.
- Дата заказа: время перехода в обработку (`InProcessAt`); интервал `[from, to)`; даты в UTC; максимальный период 366 дней.
- Выручка = цена × количество; комиссия и выплата суммируются в том виде, в каком приходят от Ozon (допущение, требует проверки на реальных данных).
- В `time-series` день определяется по московскому времени, дни без заказов возвращаются с нулями.

## Запуск локально

**Требования:** .NET SDK, PostgreSQL, Docker (для Redis).

1. Redis:
   ```
   docker run -d --name redis -p 6379:6379 redis:7
   ```
2. Секреты (в репозитории только заглушки `CHANGE_ME`; реальные значения задаются через user-secrets или переменные среды):
   ```
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<строка подключения к PostgreSQL>" --project OzonAnalytics.Api
   dotnet user-secrets set "JwtOptions:SecretKey" "<длинный случайный ключ>" --project OzonAnalytics.Api
   dotnet user-secrets set "Redis:ConnectionString" "localhost:6379" --project OzonAnalytics.Api
   ```
   Остальные параметры JWT (`Issuer`, `Audience`, `TimeAlive`) и шифрования задаются в `appsettings.json` или секретах.
3. Миграции:
   ```
   dotnet ef database update --project OzonAnalytics.Infrastructure --startup-project OzonAnalytics.Api
   ```
   Для `dotnet ef` строка подключения берётся из переменной среды `ConnectionStrings__DefaultConnection` или из секретов (см. `DBFactory`).
4. Запуск API:
   ```
   dotnet run --project OzonAnalytics.Api
   ```
   Swagger: `https://localhost:<порт>/swagger`.

### Режим без реального Ozon

В `appsettings.Development.json` включите `Ozon:UseFake`. Тогда вместо Ozon API используется `FakeOzonClient`; при подключении магазина укажите Client-Id и API-ключ `admintest`.

## Тесты

```
dotnet test
```

Сейчас покрыты маппинг статусов и сервис аналитики (права, валидация периода, сводка, сравнение, топ товаров, заполнение пропущенных дней). В планах: тесты слоя кэша, блокировки синхронизации и интеграционные тесты на Testcontainers.

## Статус и планы

Готово: аутентификация, магазины, синхронизация, блокировка в Redis, маппинг статусов, аналитика, кэш.

В планах:
- [ ] Worker: фоновая синхронизация по расписанию
- [ ] Polly: повторы при 429 и 5xx от Ozon API
- [ ] Docker Compose и CI
- [ ] Интеграционные тесты
- [ ] Реальные финансовые данные Ozon (комиссии и логистика) вместо допущений
- [ ] Замеры производительности (EF Core и SQL, влияние индексов и кэша) на 1 тыс., 10 тыс., 100 тыс. заказов
