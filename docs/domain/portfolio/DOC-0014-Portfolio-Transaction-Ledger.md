# DOC-0014 — Portfolio Transaction Ledger and Position Projection

## Decision

MarketBook persists an immutable `PortfolioTransaction` ledger as the accounting source of truth.

`Position` is not persisted as an independent source of truth in this slice. Current positions are projections rebuilt from the transaction ledger.

## Scope

This slice supports executed **Buy** and **Sell** transactions only.

Each transaction snapshots:

- PortfolioId
- ListingId
- Quote CurrencyId at execution time
- Type
- Quantity
- Price
- Commission
- Tax
- ExchangeFee
- BrokerFee
- ClearingFee
- OtherFees
- ExecutedOn
- CreatedOn

Prices, quantities and costs use `decimal(38,10)` in persistence. This avoids coupling the accounting ledger to the legacy two-decimal `Money` value object.

## Invariants

- Portfolio must exist and be active.
- Listing must exist and be active.
- Quantity > 0.
- Price > 0.
- Costs >= 0.
- Only Buy/Sell are accepted in this slice.
- A Sell cannot exceed the current open quantity.
- Transactions are append-only: no update and no delete API.

## Currency snapshot

`Listing.QuoteCurrencyId` is mutable. Therefore each transaction stores the quote currency that applied when the transaction was executed.

## Position projection

`GET /api/v1/portfolio-transactions/positions?portfolioId=...`

The projection uses weighted acquisition cost:

- Buy increases quantity and book cost by gross value + buy costs.
- Sell decreases quantity and book cost using current average acquisition cost.
- Sell fees are retained in the ledger for later realized-P/L accounting.
- Zero-quantity positions are omitted.

## Deferred

- Realized P/L
- Cash ledger
- dividends/corporate actions
- transaction corrections/reversals
- external source idempotency keys
- short positions
- multi-currency portfolio base-currency translation
- persisted/materialized Position read model
