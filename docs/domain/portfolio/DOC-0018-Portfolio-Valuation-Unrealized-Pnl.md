# DOC-0018 — Portfolio Valuation & Unrealized P/L

## Status
Foundation implementation — 2026-09-08

## Decision
Current open positions are reconstructed from the immutable `PortfolioTransaction` ledger and valued using the latest available `MarketPrice.LastPrice` for each Listing.

This slice does not persist valuation results.

## Open-position cost basis
Buy:

`OpenQuantity += Quantity`

`BookCost += GrossValue + BuyCosts`

Sell:

`AverageCost = BookCost / OpenQuantity`

`BookCost -= AverageCost × SoldQuantity`

`OpenQuantity -= SoldQuantity`

This keeps valuation consistent with the Weighted Average realized P/L foundation.

## Market valuation
For a priced open position:

`MarketValue = OpenQuantity × LatestLastPrice`

`UnrealizedPnl = MarketValue - BookCost`

`UnrealizedReturnPercent = UnrealizedPnl / BookCost × 100`

The latest market-price record is selected by descending `TradingDate`, then `CreatedOn`.

## Missing price policy
A position with no MarketPrice remains visible.

It returns:

- `IsPriced = false`
- `MarketPriceDate = null`
- `MarketPrice = null`
- `MarketValue = null`
- `UnrealizedProfitLoss = null`
- `UnrealizedReturnPercent = null`

Missing data is never silently treated as zero.

## Currency safety
The transaction ledger for one Listing must contain only one immutable Currency snapshot.

The current `Listing.QuoteCurrencyId` must also still match that transaction Currency before MarketPrice can be applied.

Cross-currency portfolio totals are deliberately not calculated in this slice.

## API
`GET /api/v1/portfolio-transactions/valuation`

Query parameters:

- `portfolioId` — required
- `listingId` — optional

## Persistence
No new entity, table, column, index, or migration is required.

## Deferred
- Portfolio-level totals by base currency.
- FX translation.
- Historical/as-of valuation.
- Intraday tick as valuation source.
- Price-source policy and fallback hierarchy.
- Materialized valuation snapshots.
- Total P/L combining realized and unrealized results.
