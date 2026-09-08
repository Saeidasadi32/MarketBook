# DOC-0017 — Portfolio Realized P/L Projection

## Status
Foundation implementation — 2026-09-08

## Decision
Realized profit/loss is reconstructed from the immutable `PortfolioTransaction` ledger and is not persisted in this slice.

The API is extensible through `PortfolioCostBasisMethod`, while the initial implementation supports only:

- `WeightedAverage = 1`

## Weighted-average rules
For Buy transactions:

`BookCost += GrossValue + BuyCosts`

For Sell transactions:

`AverageCost = BookCost / OpenQuantity`

`RelievedCostBasis = AverageCost × SoldQuantity`

`NetProceeds = GrossValue - SellCosts`

`RealizedPnl = NetProceeds - RelievedCostBasis`

Then the open position is reduced by the sold quantity and relieved cost basis.

All six transaction cost components participate through `PortfolioTransaction.TotalCosts`.

## API
`GET /api/v1/portfolio-transactions/realized-pnl`

Query parameters:

- `portfolioId` — required
- `listingId` — optional
- `costBasisMethod` — optional, default `1` (`WeightedAverage`)

## Multi-currency safety
Realized P/L is never aggregated across currencies in this slice.

If one listing contains immutable transaction snapshots in more than one currency, the projection returns:

`PortfolioTransaction.MixedCurrencyCostBasis`

FX translation must be implemented explicitly before such values can be combined safely.

## Deferred
- FIFO/LIFO/specific-lot cost basis.
- FX translation and base-currency P/L.
- Realized P/L persistence/materialization.
- Corporate actions.
- Tax-lot accounting.
- Reversal/correction workflow.
