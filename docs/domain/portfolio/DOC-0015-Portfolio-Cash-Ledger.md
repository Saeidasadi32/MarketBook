# DOC-0015 — Portfolio Cash Ledger Foundation

## Purpose
MarketBook persists portfolio cash movements as an immutable, append-only, multi-currency ledger.

## Aggregate
`PortfolioCashTransaction` stores `PortfolioId`, `CurrencyId`, `Type`, positive `Amount`, `OccurredOn`, `CreatedOn`, and optional `ReferenceType`, `ReferenceId`, and `Description`.

Amounts use `decimal(38,10)` and intentionally do not use the legacy two-decimal `Money` value object.

## Direction semantics
Credit types: `Deposit`, `SellSettlement`, `Dividend`, `Interest`, `OtherCredit`.
Debit types: `Withdrawal`, `BuySettlement`, `Fee`, `Tax`, `OtherDebit`.
The persisted `Amount` is always positive. Direction is derived from `Type`.

## Invariants
- Portfolio exists and is active.
- Currency exists and is active.
- Amount is greater than zero.
- Ledger rows are append-only; update/delete endpoints are not exposed.
- Deterministic chronological order is `OccurredOn`, then ULID `Id`.
- Negative projected cash balance is allowed in this foundation slice; overdraft/margin policy belongs to a later portfolio policy layer.

## Projection
`GET /api/v1/portfolio-cash-transactions/balances?portfolioId=...` rebuilds balances from the ledger grouped by currency. Zero balances are omitted.

## API
- `POST /api/v1/portfolio-cash-transactions`
- `GET /api/v1/portfolio-cash-transactions/{id}`
- `GET /api/v1/portfolio-cash-transactions?portfolioId=...&currencyId=...&page=...&pageSize=...`
- `GET /api/v1/portfolio-cash-transactions/balances?portfolioId=...`

## Deferred
Automatic settlement generation from Buy/Sell, idempotency/source keys, reversals/corrections, cash reservations, broker margin rules, FX translation, realized P/L, and corporate-action orchestration remain deferred.
