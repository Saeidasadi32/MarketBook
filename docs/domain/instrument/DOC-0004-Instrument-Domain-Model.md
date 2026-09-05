# Instrument Domain Model

Status date: 2026-09-05

## Purpose

`Instrument` represents the economic/security identity of a tradable financial instrument independently from where it is listed or traded. Trading-place identity belongs to `Listing`, which will reference `Instrument` and `Venue`.

## Persisted core in the first end-to-end slice

The initial persisted Instrument slice contains:

- `InstrumentId` — strongly typed ULID identifier.
- `Name` — required display name.
- `AssetClass` — required high-level asset class value object.
- `Type` — required `InstrumentType` enum.
- `Category` — required high-level instrument category value object.
- `Isin` — optional ISIN; unique when present.
- `CreatedOn`.
- `IsActive`.
- aggregate `Version` inherited from the common aggregate base.

The HTTP API supports Create, GetById, GetAll with normalized pagination, Update, Activate, and Deactivate.

## Update policy

The first Instrument slice allows updating:

- Name
- AssetClass
- Type
- Category
- ISIN

Sending a null or blank ISIN during Update removes the currently assigned ISIN.

## ISIN invariant

ISIN is optional because not every MarketBook instrument is guaranteed to have one. When present, persistence enforces uniqueness through a filtered unique index.

The current `Isin` value object validates the existing Domain format rule. This slice does not introduce a new checksum algorithm or broaden the Domain rule.

## Domain capabilities deliberately not persisted yet

The existing Domain aggregate also includes:

- Industry
- Sector
- CorporateAliases

These are intentionally ignored in the first EF mapping. They remain valid Domain capabilities, but their persistence and API contracts will be introduced in separate slices after Industry/Sector persistence rules are finalized. This avoids coupling the Instrument identity slice to unfinished classification storage.

## Relationship to Listing

Instrument does not reference Market, Exchange, Venue, or Currency directly in this slice. A future `Listing` will connect:

`Instrument -> Listing -> Venue`

and will use `Currency` for quote/pricing semantics according to the finalized Listing model.

## Verification

Automated integration tests cover:

- create and get by id
- duplicate ISIN conflict
- request validation
- update persistence and ISIN removal
- activate/deactivate idempotency
- pagination normalization
- invalid and missing identifiers

Integration tests run against the isolated SQL Server test database protected by the MarketBook integration-test database-name guard.
