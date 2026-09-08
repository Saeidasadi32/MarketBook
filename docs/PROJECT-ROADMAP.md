# MarketBook Project Roadmap

Status date: 2026-09-08

## Phase 1 â€” Market structure foundation

### Country â€” implemented

- Create
- Get by ID
- Get list
- Persistence and EF mapping
- API integration

### Exchange â€” implemented

- Create
- Get by ID
- Get list
- Update
- Deactivate
- Optional Country association
- Persistence and EF mapping
- API integration

### Market â€” implemented

- Create
- Get by ID
- Get all with pagination
- Update
- Deactivate
- Optional Exchange association
- Persistence and EF mapping
- API integration

### Venue â€” implemented

- Create
- Get by ID
- Get all with pagination
- Update Name and Type
- Activate
- Deactivate
- Required Market association
- Persistence and EF mapping
- API integration

Current Venue design intentionally keeps `VenueCode` and `MarketId` immutable after creation.

## Phase 2 â€” Listing prerequisites

Recommended order:

### 1. Currency â€” implemented and integration-tested

Reason: Listing requires `CurrencyId` as quote currency.

Implemented slice:

- Create Currency
- GetById
- GetAll with pagination
- Update Code, Name, and DecimalPlaces
- Activate / Deactivate
- EF configuration, repository, migration, and API endpoints

Automated SQL Server integration tests are passing for the Currency slice.

### 2. Instrument â€” implemented and integration-tested

Reason: Listing requires `InstrumentId`.

Implemented slice:

- Create Instrument
- GetById
- GetAll with pagination
- Update Name, AssetClass, Type, Category, and optional ISIN
- Activate / Deactivate
- Optional unique ISIN
- EF configuration, repository, migration, and API endpoints
- Automated SQL Server integration tests

Industry, Sector, and CorporateAliases remain Domain capabilities but are intentionally not persisted in this first Instrument slice.

### 3. Listing â€” implemented and integration-tested

Listing depends on:

- Instrument
- Venue
- Currency

Before implementation, reconcile and finalize Listing invariants, especially trading-symbol uniqueness scope, tick size, precision, primary-listing behavior, and activation lifecycle.

Target slice:

- Create Listing
- GetById
- GetAll / filter by Venue and Instrument
- Activate / Deactivate
- Primary listing operations
- EF configuration and indexes
- API and integration tests

## Phase 3 â€” Trading calendar and market structure details

### TradingCalendar / TradingSession â€” implemented and integration-tested

Implemented:

- yearly calendar per Market
- configurable weekend days (no Saturday/Sunday hard-code)
- regular weekly TradingSession open/close times
- holiday and half-day date exceptions
- day evaluation endpoint
- Create / GetById / GetAll / Update
- Activate / Deactivate
- EF mapping, migration, relational constraints, and integration tests

### MarketPrice / price limits â€” implemented and integration-tested

- official daily OHLC and close/reference prices
- daily permitted lower/upper price limits
- unique Listing/date record
- persistence, API, migration, and integration tests

## Phase 4 â€” Market data

- Daily prices â€” fulfilled by MarketPrice daily record; implemented and integration-tested
- Daily trade statistics â€” implemented and integration-tested
- Intraday/tick snapshots â€” implemented by IntradayPriceTick and integration-tested
- Order book â€” implemented by OrderBookSnapshot and integration-tested

Only after Listing identity and lifecycle are stable.

## Phase 5 â€” User-facing portfolio capabilities

- Investor foundation - implementation patch prepared; pending user-environment verification

- Watchlist
- Portfolio
- Position
- Trade
- Portfolio events

Reference identity rules must be aligned with the finalized Listing model before these slices are persisted.

## Cross-cutting work

These items should proceed alongside feature development:

- Continue expanding the existing automated integration-test suite for every new end-to-end feature.
- Standardize command/query abstractions (either direct `IRequest` or project `ICommand`/`IQuery`) before broad expansion.
- Standardize bilingual XML documentation style.
- Keep pagination normalized through `PageRequest` and `PagedResult`.
- Keep API errors centralized through `ApiErrorMapper`.
- Add concurrency strategy only when a concrete aggregate requires it; do not introduce broad infrastructure prematurely.
