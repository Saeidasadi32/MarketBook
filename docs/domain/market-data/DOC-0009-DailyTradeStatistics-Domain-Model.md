# DOC-0009 — Daily Trade Statistics Domain Model

Status: implementation patch prepared

## Purpose

`DailyTradeStatistics` stores the official daily trading statistics for one `Listing` and one trading date without duplicating OHLC and price-limit data already owned by `MarketPrice`.

## Identity and uniqueness

- Strongly typed ULID: `DailyTradeStatisticsId`
- Required `ListingId`
- Immutable `TradingDate`
- Unique database key: `(ListingId, TradingDate)`

## Persisted fields

- Volume
- TradeCount
- AveragePrice
- TradeValue
- MarketCapitalization
- CreatedOn
- UpdatedOn

## Invariants

All numeric statistics must be non-negative. `ListingId` and `TradingDate` are immutable after creation.

## Integration boundary

The Application layer requires the referenced Listing to exist and be active on create. A corresponding `MarketPrice` record is intentionally not required because price feeds and statistics feeds may arrive independently.
