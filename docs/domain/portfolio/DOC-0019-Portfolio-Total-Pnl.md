# DOC-0019 — Portfolio Total P/L

## Status
Foundation implementation — 2026-09-08

## Decision
Portfolio total P/L is reconstructed from the immutable `PortfolioTransaction` ledger and latest available `MarketPrice` records.

No result is persisted in this slice.

## Cost basis
This slice uses the same moving Weighted Average cost basis as realized P/L and valuation.

For Buy:

`BookCost += GrossValue + BuyCosts`

For Sell:

`AverageCost = BookCost / OpenQuantity`

`RelievedCostBasis = AverageCost × SoldQuantity`

`RealizedPnl += (GrossValue - SellCosts) - RelievedCostBasis`

`BookCost -= RelievedCostBasis`

## Unrealized and total P/L
For an open priced position:

`MarketValue = OpenQuantity × LatestLastPrice`

`UnrealizedPnl = MarketValue - RemainingBookCost`

`TotalPnl = RealizedPnl + UnrealizedPnl`

For a fully closed position:

`UnrealizedPnl = 0`

`TotalPnl = RealizedPnl`

For an open unpriced position:

- realized P/L remains valid;
- unrealized P/L is `null`;
- total P/L is `null`.

Missing market data is never interpreted as zero.

## Currency summaries
The response contains summaries grouped by Currency.

Cross-currency aggregation is forbidden in this slice.

A currency summary always exposes realized P/L.

`UnrealizedProfitLoss` and `TotalProfitLoss` are returned only when every open position in that currency has a market price.

Otherwise:

`IsFullyPriced = false`

`UnrealizedProfitLoss = null`

`TotalProfitLoss = null`

This prevents partial valuation from appearing as a complete portfolio result.

## Currency safety
Each Listing ledger must contain one immutable transaction currency.

For any open position, the current `Listing.QuoteCurrencyId` must match that transaction currency before MarketPrice can be applied.

## API
`GET /api/v1/portfolio-transactions/total-pnl`

Query parameters:

- `portfolioId` — required
- `listingId` — optional

## Persistence
No entity, table, column, index, or migration is introduced.

## Deferred
- Portfolio base currency.
- FX conversion and translated totals.
- Historical/as-of total P/L.
- Corporate actions.
- Materialized performance snapshots.
- Time-weighted return (TWR).
- Money-weighted return / IRR.
