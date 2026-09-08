# DOC-0010 — IntradayPriceTick Domain Model

Status date: 2026-09-08

## Purpose

`IntradayPriceTick` represents one immutable executed-trade price observation for a `Listing`.

It does not contain order-book bid/ask levels. Order-book state belongs to a separate future aggregate such as `OrderBookSnapshot`.

## Identity and uniqueness

Each tick has a ULID identifier.

Business uniqueness is:

`(ListingId, TradingDate, SequenceNumber)`

The sequence number is intentionally scoped to a Listing and trading date so multiple executions with identical timestamps remain representable.

## Fields

- `ListingId`
- `TradingDate`
- `OccurredAt`
- `SequenceNumber`
- `Price`
- `Volume`
- computed `TradeValue`
- `CreatedOn`

## Invariants

- Listing is required.
- Sequence number cannot be negative.
- Price must be greater than zero.
- Volume must be greater than zero.
- Ticks are append-only and have no update operation.
- Application creation requires an existing active Listing.

`TradingDate` is stored explicitly rather than inferred from `OccurredAt`, because an exchange trading day does not always equal the calendar date represented by a timestamp in a generic multi-market platform.

## Persistence

Primary query shape:

`ListingId + TradingDate -> OccurredAt + SequenceNumber`

Indexes:

- unique `(ListingId, TradingDate, SequenceNumber)`
- query index `(ListingId, TradingDate, OccurredAt, SequenceNumber)`

Delete behavior from Listing is restricted.

## Initial API slice

- `POST /api/v1/intraday-price-ticks`
- `GET /api/v1/intraday-price-ticks/{id}`
- `GET /api/v1/intraday-price-ticks?listingId=...&tradingDate=...&page=...&pageSize=...`

Bulk ingestion and cursor/keyset pagination are intentionally deferred to the ingestion/performance slice. The first slice keeps the established `PageRequest` contract while preserving an index shape that supports a later cursor strategy.
