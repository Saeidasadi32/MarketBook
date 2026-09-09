---
id: DOC-0022
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Portfolio Base-Currency NAV Translation
version: 1.0.0
---

# Portfolio Base-Currency NAV Translation
# ترجمه NAV پرتفوی به ارز پایه

## 1. Purpose / هدف

EN: This slice translates the current per-currency NAV projection into the portfolio's configured reporting/base currency without directly summing unlike currencies.

FA: این Slice، NAV جاری به تفکیک ارز را به ارز پایه/گزارش‌دهی تنظیم‌شده پرتفوی ترجمه می‌کند، بدون آن‌که ارزهای متفاوت مستقیماً با هم جمع شوند.

## 2. Endpoint

`GET /api/v1/portfolios/{portfolioId}/translated-nav`

## 3. Source Projection / منبع محاسبه

The feature reuses `GetPortfolioNavQuery`.

For each source currency:

`SourcePricedNAV = CashBalance + PricedMarketValue`

`SourceNAV = SourcePricedNAV` only when every open position in that currency is priced; otherwise `null`.

The translated handler does not rebuild transaction positions or cash balances independently.

## 4. FX Convention / قرارداد FX

Stored convention:

`1 BaseCurrency = Rate × QuoteCurrency`

For source currency `S` and portfolio base currency `B`:

- if `S == B`, translation rate is `1`;
- if stored pair is `S -> B`, `SourceToBaseRate = Rate`;
- if stored pair is `B -> S`, `SourceToBaseRate = 1 / Rate`.

The newest available direct/inverse pair is selected by:

1. `RateDate DESC`
2. `CreatedOn DESC`

No triangulation is performed in this slice.

## 5. Translation Formulas / فرمول‌ها

For an available FX rate `R`:

`CashBalanceBase = SourceCashBalance × R`

`PricedMarketValueBase = SourcePricedMarketValue × R`

`PricedNetAssetValueBase = SourcePricedNetAssetValue × R`

`NetAssetValueBase = SourceNetAssetValue × R`

The last value remains `null` when the source NAV is incomplete.

## 6. Completeness Semantics / معنای کامل‌بودن

### IsPricedNavFullyTranslated

True when every source-currency NAV row has an FX path to the configured base currency.

When false:

`PricedNetAssetValueBase = null`

at portfolio aggregate level.

### IsComplete

True only when:

- every source currency has FX; and
- every open position in every source currency is priced.

When false:

`NetAssetValueBase = null`

at portfolio aggregate level.

This prevents missing prices or missing FX from being silently treated as zero.

## 7. Missing FX / نبود نرخ ارز

Missing FX is not an endpoint failure.

The source NAV row remains visible, but translated monetary fields for that currency are `null` and aggregate translated NAV is incomplete.

## 8. Base Currency Identity / تبدیل همانی ارز پایه

Assets already denominated in the portfolio base currency use:

`SourceToBaseRate = 1`

and do not require an `FxRate` row.

`FxRateDate` is `null` for identity conversion.

## 9. Persistence / ماندگاری

No new table, column, index, or migration is introduced.

This feature is a read-only projection over:

- Portfolio base currency;
- current NAV by currency;
- current FX-rate records.

## 10. Deferred / موارد موکول‌شده

- historical/as-of NAV;
- historical/as-of FX policy;
- FX source/provider metadata;
- bid/ask/mid policy;
- triangulation through a third currency;
- market/official FX source precedence;
- stale-rate policy;
- cash-flow timing and performance analytics;
- TWR / XIRR;
- FX gains and losses;
- persisted NAV snapshots.

## 11. Safety / ایمنی

Integration-test database safety guards are not modified by this slice.

Missing data is represented explicitly through nullable translated totals and completeness flags rather than fabricated zero values.
