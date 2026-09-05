# MarketBook Documentation

This directory contains architecture, domain, business, implementation-status, and development-convention documents for MarketBook.

## Architecture

- `architecture/principles/ARCH-0001-Architecture-Principles.md` — dependency rules, DDD/CQRS boundaries, and persistence principles.
- `architecture/adr/ADR-0002-Identifier-Strategy.md` — identifier strategy.
- `architecture/adr/ADR-0003-Trading-Hierarchy.md` — Exchange, Market, Venue, and Listing boundaries.

## Business and Domain

- `business/BUS-0003-Instrument.md`
- `business/BUS-0004-Market.md`
- `business/BUS-0005-Listing.md`
- `domain/venue/DOC-0005-Venue-Domain-Model.md`

Some other domain documents are still drafts/placeholders and should be expanded when their corresponding end-to-end slice is implemented.

## Development

- `development/DEV-0001-IMPLEMENTATION-CONVENTIONS.md` — conventions derived from the current working Country/Exchange/Market/Venue implementation.
- `status/IMPLEMENTATION-STATUS.md` — what is actually implemented versus domain-only.
- `PROJECT-ROADMAP.md` — recommended next implementation order.

## Documentation rule

A Domain aggregate may exist before persistence/API implementation. Documentation must explicitly distinguish:

1. **Domain modeled** — classes and invariants exist in `MarketBook.Domain`.
2. **End-to-end implemented** — Application, Infrastructure, EF mapping/repository, API, migration, and tests exist.

This distinction prevents Domain prototypes from being mistaken for completed platform capabilities.
