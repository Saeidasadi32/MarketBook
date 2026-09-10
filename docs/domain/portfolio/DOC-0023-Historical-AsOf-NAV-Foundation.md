---
id: DOC-0023
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Historical / As-Of NAV Foundation
version: 1.0.0
---

# Historical / As-Of NAV Foundation
# پایه NAV تاریخی / As-Of

## 1. Purpose / هدف

EN: Reconstruct portfolio NAV at an explicit historical instant from immutable portfolio ledgers and historical market prices.

FA: NAV پرتفوی در یک لحظه تاریخی مشخص، از روی Ledgerهای تغییرناپذیر و قیمت‌های تاریخی بازار بازسازی می‌شود.

## 2. Endpoint

`GET /api/v1/portfolios/{portfolioId}/nav/as-of?asOf=<DateTimeOffset>`

The cutoff is inclusive.

## 3. Ledger Cutoff / برش Ledger

Only portfolio transactions satisfying:

`ExecutedOn <= AsOf`

participate in position reconstruction.

Only cash transactions satisfying:

`OccurredOn <= AsOf`

participate in cash-balance reconstruction.

Automatic trade settlements naturally follow the trade's `ExecutedOn` because settlement `OccurredOn` is copied from it.

## 4. Historical Market Price / قیمت تاریخی بازار

For every open position as of the cutoff, valuation uses the latest available `MarketPrice` satisfying:

`TradingDate <= DateOnly(AsOf)`

ordered by:

1. `TradingDate DESC`
2. `CreatedOn DESC`

A market price after the cutoff date is never backfilled into the historical NAV.

## 5. Current Behavior Preservation / حفظ رفتار جاری

Existing current queries remain current when `AsOf = null`.

Historical support is added as an optional cutoff to the underlying cash-balance and valuation projections, avoiding a separate duplicate accounting implementation.

## 6. NAV Semantics / معنای NAV

Per currency:

`PricedNAV = CashBalance + PricedMarketValue`

If all open positions have an eligible historical price:

`NAV = PricedNAV`

Otherwise:

`NAV = null`

and `PricedNAV` remains visible.

No cross-currency aggregation is performed.

## 7. Time Policy / سیاست زمان

`AsOf` is represented as `DateTimeOffset` to preserve an explicit instant for ledger filtering.

For daily `MarketPrice`, the date component of the supplied `AsOf` value is used as the trading-date ceiling.

This Slice does not yet introduce exchange-session timezone normalization. That policy is intentionally deferred rather than silently inferred.

## 8. Persistence / ماندگاری

No schema change and no migration.

The feature reads existing:

- PortfolioTransaction
- PortfolioCashTransaction
- MarketPrice
- Listing
- Portfolio

## 9. Deferred / موارد موکول‌شده

- exchange-local trading-day normalization;
- historical FX translation;
- historical base-currency NAV;
- stale-price policy;
- corporate actions;
- split/dividend-adjusted historical series;
- persisted NAV snapshots;
- period return;
- cash-flow classification for performance;
- TWR;
- XIRR / money-weighted return.

## 10. Safety / ایمنی

Integration-test database safety guards are not modified.

Future ledger entries and future market prices are excluded instead of being implicitly incorporated into historical results.
