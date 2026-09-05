# MarketBook Instrument Patch — 2026-09-05

This patch is based on the user-provided `MarketBook(9).zip` snapshot plus the verified Integration Tests SafetyFix 2.

## Included

- Instrument Domain refinements required for update operations.
- `IInstrumentRepository`.
- `InstrumentRepository`.
- `InstrumentConfiguration`.
- DI registration.
- `20260905210000_AddInstrument` migration and updated model snapshot.
- Instrument Create / Update / GetById / GetAll / Activate / Deactivate slices.
- `InstrumentsController` under `/api/v1/instruments`.
- Automated SQL Server integration tests.
- Safe `ResetInstrumentsAsync()` using the existing integration database guard.
- Instrument Domain documentation and roadmap/status updates.

## Persistence scope

The first persisted Instrument slice includes Name, AssetClass, Type, Category, optional unique ISIN, CreatedOn, IsActive, and Version.

Existing Domain properties Industry, Sector, and CorporateAliases are deliberately ignored by EF in this slice and are documented for later persistence work.

## Safety

Integration tests retain the verified SafetyFix 2 behavior:

- isolated per-run database name starting with `MarketBookIntegrationTestsDb_`
- explicit ApplicationDbContext replacement in the test host
- safety guard before destructive database operations
- no initialization-time `EnsureDeletedAsync()`

## Coding convention

No `var` is used in new or modified Instrument/Test C# files in this patch.

## Apply

Extract this ZIP over the MarketBook repository root and replace matching files.

Then run:

```powershell
dotnet build

dotnet ef database update `
  --project src/MarketBook.Infrastructure `
  --startup-project src/MarketBook.Api

dotnet test tests/MarketBook.Integration.Tests/MarketBook.Integration.Tests.csproj
```

Migration: `20260905210000_AddInstrument`
