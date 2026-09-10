---
id: DOC-0024
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Historical Base-Currency NAV and FX Cutoff
version: 1.0.0
---

# Historical Base-Currency NAV and FX Cutoff
# NAV تاریخی در ارز پایه و برش تاریخی FX

## 1. Purpose / هدف

EN: Translate historical per-currency portfolio NAV into the configured portfolio base currency without allowing future FX rates to leak into the past.

FA: NAV تاریخی پرتفوی به تفکیک ارز به ارز پایه تنظیم‌شده ترجمه می‌شود، بدون اینکه نرخ‌های FX آینده وارد محاسبات گذشته شوند.

## 2. Endpoint

`GET /api/v1/portfolios/{portfolioId}/translated-nav/as-of?asOf=<DateTimeOffset>`

The cutoff is inclusive.

## 3. Source NAV / NAV مبدا

The feature composes `GetPortfolioNavAsOfQuery`.

Therefore the source NAV already obeys:

- `PortfolioTransaction.ExecutedOn <= AsOf`
- `PortfolioCashTransaction.OccurredOn <= AsOf`
- latest `MarketPrice.TradingDate <= DateOnly(AsOf)`

No current/future source ledger entry or market price is introduced by the translation layer.

## 4. Historical FX / FX تاریخی

For a source currency different from the portfolio base currency, only FX rows satisfying:

`FxRate.RateDate <= DateOnly(AsOf)`

are eligible.

The selected row is ordered by:

1. `RateDate DESC`
2. `CreatedOn DESC`

Future FX rates are never used for historical NAV.

## 5. FX Direction / جهت نرخ

Stored convention:

`1 BaseCurrency = Rate × QuoteCurrency`

If the stored pair is:

`Source -> PortfolioBase`

then:

`SourceToBaseRate = Rate`

If the stored pair is:

`PortfolioBase -> Source`

then:

`SourceToBaseRate = 1 / Rate`

If source currency equals portfolio base currency:

`SourceToBaseRate = 1`

and no persisted FX row is required.

## 6. Translation / ترجمه

Per source currency:

`CashBalanceBase = SourceCashBalance × SourceToBaseRate`

`PricedMarketValueBase = SourcePricedMarketValue × SourceToBaseRate`

`PricedNAVBase = SourcePricedNAV × SourceToBaseRate`

When source NAV is complete:

`NAVBase = SourceNAV × SourceToBaseRate`

## 7. Completeness / کامل‌بودن

`IsPricedNavFullyTranslated = true`

only when every source currency has an eligible FX conversion.

`IsComplete = true`

only when:

- every source currency has eligible FX, and
- every source NAV is fully priced.

If historical FX is missing, source values remain visible but translated monetary values for that currency are null.

If historical market pricing is incomplete but FX exists, translated `PricedNAV` remains available while complete `NAV` is null.

Missing price and missing FX are never treated as zero.

## 8. Aggregation / تجمیع

Cross-currency source values are never added directly.

Aggregate base-currency `PricedNetAssetValueBase` exists only when every source currency can be translated.

Aggregate base-currency `NetAssetValueBase` exists only when every source currency can be translated and every source NAV is complete.

## 9. Time Semantics / معنای زمان

Ledger filtering uses the exact `DateTimeOffset AsOf` instant.

Daily MarketPrice and FxRate selection use the date component carried by the supplied `AsOf` value.

Exchange-local timezone normalization remains deferred and is not silently inferred.

## 10. Persistence / ماندگاری

No schema change.
No migration.
No NAV snapshot is persisted.

## 11. Deferred / موارد موکول‌شده

- shared reusable FX conversion policy/service;
- exchange-local trading-day normalization;
- FX triangulation;
- stale-rate policy;
- official versus market FX precedence;
- bid/ask/mid selection;
- corporate actions;
- historical NAV snapshots;
- external cash-flow classification;
- period return;
- TWR;
- XIRR / money-weighted return.

## 12. Safety / ایمنی

Integration-test database safety guards are not modified.
