# Implementation Status

Status date: 2026-09-05

## End-to-end implemented

| Aggregate | Domain | Application | EF/Repository | Migration | API | Manual verification |
|---|---|---|---|---|---|---|
| Country | Yes | Yes | Yes | Yes | Yes | Existing flow |
| Exchange | Yes | Yes | Yes | Yes | Yes | Existing flow |
| Market | Yes | Yes | Yes | Yes | Yes | Passed |
| Venue | Yes | Yes | Yes | Yes | Yes | Passed |
| Currency | Yes | Yes | Yes | Yes | Yes | Automated integration tests passed |
| Instrument | Yes | Yes | Yes | Yes | Yes | Pending build/test on user environment |

Venue manual verification covers successful and error cases for Create, GetById, GetAll/pagination, Update, Activate, and Deactivate. Currency automated integration tests pass against the isolated SQL Server test database. Instrument is implemented end-to-end in this patch and awaits build/test verification on the user environment.

## Domain-modeled but not end-to-end implemented

The source tree currently contains Domain types for areas including:

- Listing
- Calendar
- MarketData
- Portfolio
- Watchlist
- CorporateActions
- Industry
- Investor
- Sector and financial value objects

These must not be described as complete platform features until persistence, Application slices, API contracts, migrations, and tests are implemented.

## Test-project status

The existing `MarketBook.Integration.Tests` project now contains real endpoint coverage for Currency and Instrument. Tests run through the real API/EF/SQL Server pipeline against an isolated per-run test database. A database-name safety guard prevents destructive test operations against `MarketBookDb` or any database without the integration-test prefix.

Remaining test priorities:

1. Add regression integration coverage for Country, Exchange, Market, and Venue.
2. Add focused Domain tests for strongly typed IDs/value objects and aggregate invariants where useful.
3. Extend integration coverage with Listing foreign-key and uniqueness scenarios once Listing is implemented.

## Static architecture review findings

### Good foundations

- Correct inward dependency direction across Domain/Application/Infrastructure/API.
- Strongly typed ULID identifiers.
- Explicit repository abstractions.
- Centralized API error mapping.
- Server-side pagination pattern for Market/Venue.
- Venue -> Market foreign key with restricted delete behavior.
- Clear activation/deactivation lifecycle.

### Items to address deliberately

- Direct MediatR `IRequest<T>` and custom `ICommand`/`IQuery` abstractions coexist. Standardize later through one ADR rather than piecemeal edits.
- XML documentation style is not fully uniform across older and newer files.
- Some Domain areas are prototypes and may require invariant review before persistence.
- TradingCalendar currently assumes Saturday/Sunday weekends; this is not globally correct for every market and should become market/calendar configuration before production use.
- Listing documentation previously referred directly to Market while code references Venue; documentation has been aligned to the current model in this snapshot.

## Build verification note

This reviewed package was based on the user-provided snapshot that had already been reported as building successfully. The review environment used to prepare these documentation updates did not contain the .NET SDK, so no additional `dotnet build` was executed here. No production C# behavior was changed as part of this review package.
