# MarketBook Listing Patch — 2026-09-05

This patch is based on the current green MarketBook state:
- `MarketBook(9).zip`
- Integration Tests SafetyFix 2
- verified Instrument patch

## Listing architecture

`Listing` connects:
- Instrument
- Venue
- Quote Currency

Finalized invariants:
- `InstrumentId` and `VenueId` are immutable after Create.
- Trading symbol uniqueness scope: `(VenueId, TradingSymbol)`.
- One Instrument can have at most one primary Listing.
- TickSize > 0.
- PricePrecision is 0..28.
- Foreign keys to Instrument, Venue, Currency use Restrict delete behavior.

## API

`/api/v1/listings`

Includes:
- POST Create
- GET by id
- GET paged
- PUT Update
- PATCH activate/deactivate
- PATCH make-primary/remove-primary

## Migration

`20260905220000_AddListing`

## Integration safety

The verified integration-test database guard remains in force.
Listing test reset deletes dependent rows before parent rows and refuses mutation unless the active database name begins with:

`MarketBookIntegrationTestsDb_`

## Apply

Extract this ZIP at repository root and replace matching files.

Run:

```powershell
dotnet build

dotnet ef database update `
  --project src/MarketBook.Infrastructure `
  --startup-project src/MarketBook.Api

dotnet test tests\MarketBook.Integration.Tests\MarketBook.Integration.Tests.csproj
```
