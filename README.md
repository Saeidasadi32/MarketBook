# MarketBook

MarketBook (Intelligent Market Book System) is a .NET 10 platform for modeling markets, trading venues, financial instruments, listings, market data, portfolios, watchlists, and related financial-domain concepts.

## Architecture

The solution follows a layered architecture with Domain-Driven Design and CQRS/Vertical Slice patterns:

- **MarketBook.Domain** — aggregates, entities, value objects, domain events, and business invariants.
- **MarketBook.Application** — use cases, MediatR commands/queries, validation, repository abstractions, pagination, and application behaviors.
- **MarketBook.Infrastructure** — EF Core persistence, SQL Server mappings, repositories, and infrastructure services.
- **MarketBook.Api** — HTTP API endpoints and error-to-HTTP mapping.
- **MarketBook.Web** — web host/UI project.
- **tests** — Domain, Application, and Integration test projects.

## Implemented end-to-end slices

The following aggregates currently have Application + Infrastructure + API support:

- Country
- Exchange
- Market
- Venue
- Currency

Venue and Currency currently support Create, GetById, GetAll with pagination, Update, Activate, and Deactivate.

The Domain layer contains additional models that are not yet fully persisted/exposed through the API. Their presence in Domain does **not** mean the corresponding feature is production-complete.

## Trading hierarchy

The current model distinguishes these concepts:

`Exchange (optional organizational parent) -> Market (logical market) -> Venue (concrete trading venue) -> Listing (instrument traded at a venue)`

A Listing references a Venue, an Instrument, and a quote Currency.

## Technology

- .NET 10
- ASP.NET Core
- EF Core 10
- SQL Server
- MediatR
- FluentValidation
- NUlid
- xUnit

## Documentation

Start at [docs/README.md](docs/README.md). The current implementation status is documented in [docs/status/IMPLEMENTATION-STATUS.md](docs/status/IMPLEMENTATION-STATUS.md), and the development sequence is maintained in [docs/PROJECT-ROADMAP.md](docs/PROJECT-ROADMAP.md).
