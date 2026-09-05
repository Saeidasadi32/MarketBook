# DOC-0006 — Listing Domain Model

Status date: 2026-09-05

## Purpose

`Listing` is the tradable manifestation of an `Instrument` on one concrete `Venue`.

The hierarchy is:

`Instrument -> Listing -> Venue`

and every Listing also references a quote `Currency`.

## Identity and immutable links

A Listing has a strongly typed ULID `ListingId`.

The following relationships are creation-time identity links and are intentionally immutable after creation:

- `InstrumentId`
- `VenueId`

Changing either would semantically create a different Listing rather than edit the existing one.

## Mutable attributes

The following values can change:

- `QuoteCurrencyId`
- `TradingSymbol`
- `TickSize`
- `PricePrecision`
- active state
- primary state

## Trading symbol uniqueness

`TradingSymbol` is normalized to uppercase by the Domain value object.

Uniqueness scope is finalized as:

`(VenueId, TradingSymbol)`

This allows the same symbol to exist on different Venues while preventing ambiguity inside one Venue.

## Pricing invariants

- TickSize > 0
- PricePrecision: 0..28
- SQL storage for TickSize: `decimal(38,28)`

`RoundPrice` remains a Domain behavior using TickSize and PricePrecision.

## Primary listing invariant

An Instrument may have many Listings but at most one primary Listing.

The Application handler moves primary status by clearing the previous primary Listing before setting the new one.

SQL Server additionally protects this invariant with:

`UX_Listings_Instrument_Primary`

a filtered unique index on `InstrumentId` where `IsPrimary = 1`.

## Referential integrity

Listing has restrictive foreign keys to:

- Instrument
- Venue
- Currency

Create rejects missing or inactive referenced entities.

## API slice

`/api/v1/listings`

supports:

- POST Create
- GET by id
- GET paged
- PUT mutable attributes
- PATCH activate
- PATCH deactivate
- PATCH make-primary
- PATCH remove-primary

## Integration coverage

Automated SQL Server integration tests verify:

- create/get
- venue-scoped duplicate symbol rejection
- invalid symbol/tick/precision
- missing referenced Instrument
- update persistence
- activation/deactivation idempotency
- primary-listing switching and idempotency
- pagination normalization
- invalid/missing Listing IDs

All destructive test operations continue to use the isolated integration database name guard.
