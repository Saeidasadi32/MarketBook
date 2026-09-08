# DOC-0011 — OrderBookSnapshot Domain Model

## Purpose

`OrderBookSnapshot` is the persisted, historical depth-of-market record for one `Listing` at one source capture point.
It is intentionally separate from `IntradayPriceTick`: ticks represent executed trades, while order-book snapshots represent resting bid/ask liquidity.

## Identity and time

Each snapshot stores:

- `OrderBookSnapshotId` (ULID)
- `ListingId`
- logical `TradingDate`
- `CapturedAt` (`DateTimeOffset`)
- source `SequenceNumber`
- ordered depth `Levels`
- persistence `CreatedOn`

Uniqueness is `(ListingId, TradingDate, SequenceNumber)`. The sequence number is preferred over timestamp-only identity because several source updates can share the same timestamp resolution.

## Level model

Each level has a one-based `Level` and optional Bid/Ask sides. A populated side must provide all three values together:

- price
- volume
- order count

A snapshot may contain a Bid-only or Ask-only level, which is needed for thin or one-sided markets. At least one side must exist per level.

## Invariants

- Snapshot sequence number cannot be negative.
- At least one level is required.
- Levels are unique and contiguous beginning at 1.
- Bid prices may stay equal or decrease as depth increases; they may not increase.
- Ask prices may stay equal or increase as depth increases; they may not decrease.
- Populated prices must be greater than zero.
- Populated volumes and order counts cannot be negative.
- Listing must exist and be active at creation.
- Snapshot is immutable after persistence.

The model deliberately does **not** reject locked/crossed books (`best bid >= best ask`). Such states can occur transiently or be emitted by source systems; ingestion should preserve source truth and analytics can classify the condition separately.

## Persistence

Tables:

- `OrderBookSnapshots`
- `OrderBookSnapshotLevels`

Indexes:

- unique `(ListingId, TradingDate, SequenceNumber)`
- query `(ListingId, TradingDate, CapturedAt, SequenceNumber)`

The FK from snapshots to `Listings` uses Restrict / NO ACTION.

## API slice

- `POST /api/v1/order-book-snapshots`
- `GET /api/v1/order-book-snapshots/{id}`
- `GET /api/v1/order-book-snapshots?listingId=...&tradingDate=...&page=...&pageSize=...`

This first slice uses the existing page-number contract for consistency. High-volume ingestion should later add batch/bulk writes, cursor/keyset queries, retention rules, and partitioning strategy.

## Relationship to legacy domain prototype

The repository already contains domain-only `MarketData.Entities.OrderBook` / `OrderBookLevel`, referenced by `DailySnapshot`. This patch does not delete or silently repurpose that prototype. Persisted intraday depth uses the explicit `OrderBookSnapshot` name so the legacy end-of-day composition can be refactored deliberately in a later cleanup/ADR rather than broken incidentally.
