# MarketBook Trading Calendar / Session Patch — 2026-09-06

Based directly on the user-provided green `MarketBook(10).zip`.

## Main change

The old TradingCalendar prototype hard-coded Saturday/Sunday as weekends.

This patch removes that assumption and makes weekend days a persisted per-calendar configuration.

## Included

- configurable `TradingWeekDays` bit flags
- completed `TradingCalendar` aggregate
- completed regular weekly `TradingSession`
- persisted holiday / half-day exceptions
- one calendar per `(MarketId, Year)`
- one regular session per calendar/day
- Create / GetById / GetAll / Update
- Activate / Deactivate
- day-evaluation endpoint
- repository + EF configuration + DI
- migration `20260906210000_AddTradingCalendar`
- SQL Server integration tests
- docs / roadmap / implementation status

## API

`/api/v1/trading-calendars`

Day evaluation example:

`GET /api/v1/trading-calendars/{id}/days/2026-09-05`

## Safety

The existing isolated integration-test database guard is retained.

Calendar reset also respects FK order and deletes TradingCalendar before parent Market records.

## Coding convention

No `var` or `out var` is used in new/modified C# files in this patch.

## Run

```powershell
dotnet build

dotnet ef database update `
  --project src/MarketBook.Infrastructure `
  --startup-project src/MarketBook.Api

dotnet test tests\MarketBook.Integration.Tests\MarketBook.Integration.Tests.csproj
```
