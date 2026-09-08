# DOC-0016 — Portfolio Trade Cash Settlement

## Status
Foundation implementation — 2026-09-08

## Decision
Creating a `PortfolioTransaction` also creates its cash settlement in the same EF Core unit of work.

## Rules
- Buy settlement = `Quantity × Price + all six transaction-cost components`.
- Sell settlement = `Quantity × Price - all six transaction-cost components`.
- Buy creates `BuySettlement`; Sell creates `SellSettlement`.
- Currency is copied from the immutable `PortfolioTransaction.CurrencyId` snapshot.
- `OccurredOn` equals `ExecutedOn`.
- `ReferenceType` is `PortfolioTransaction`.
- `ReferenceId` is the source transaction id.
- A filtered unique index on `(ReferenceType, ReferenceId)` prevents duplicate source settlement.
- A sell whose costs consume or exceed gross proceeds is rejected.

## Atomicity
Both ledger rows are added before a single `SaveChangesAsync`. The application does not intentionally persist a trade without its settlement or a settlement without its trade.

## Deferred
Corrections/reversals, T+N settlement dates, partial settlement, FX conversion, broker receivable/payable, and realized P/L.
