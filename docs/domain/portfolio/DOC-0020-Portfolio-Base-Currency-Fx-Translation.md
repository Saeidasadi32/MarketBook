# DOC-0020 — Portfolio Base Currency and FX Translation

## Status
Foundation implementation — 2026-09-08

## Portfolio base currency
`Portfolio.BaseCurrencyId` is an optional persisted reporting currency.

It is nullable for backward compatibility with existing portfolios and existing production/test data.

A portfolio can set an active Currency as its base currency through:

`PATCH /api/v1/portfolios/{id}/base-currency`

Body:

```json
{
  "currencyId": "<CurrencyId>"
}
```

## FX quote convention
`FxRate` stores a dated directional quote:

`1 BaseCurrency = Rate × QuoteCurrency`

Example:

`1 EUR = 2 USD`

means:

- BaseCurrency = EUR
- QuoteCurrency = USD
- Rate = 2

FX rates are persisted with:

- Id
- BaseCurrencyId
- QuoteCurrencyId
- RateDate
- Rate
- CreatedOn

The tuple `(BaseCurrencyId, QuoteCurrencyId, RateDate)` is unique.

Rate precision:

`decimal(38,10)`

## Translation rule
To translate a source amount into the portfolio base currency, MarketBook searches the newest available rate between the two currencies in either direction.

If the stored direction is:

`Source -> PortfolioBase`

then:

`SourceToBaseRate = Rate`

If the stored direction is:

`PortfolioBase -> Source`

then:

`SourceToBaseRate = 1 / Rate`

The newest quote is selected by:

1. RateDate descending
2. CreatedOn descending

If source currency already equals portfolio base currency:

`SourceToBaseRate = 1`

and no FX record is required.

## Translated total P/L
Endpoint:

`GET /api/v1/portfolio-transactions/translated-pnl?portfolioId=...&listingId=...`

The endpoint builds on the existing per-currency Total P/L projection.

It never adds different currencies before translation.

For each source currency:

`TranslatedAmount = SourceAmount × SourceToBaseRate`

## Completeness semantics
`IsRealizedFullyTranslated` is true only when every source currency has a usable translation rate.

`IsFullyPricedAndTranslated` is true only when:

- every source currency has a usable FX rate, and
- every open position is fully priced.

If any required FX rate is missing:

- the affected currency remains visible;
- translated fields for that currency are null;
- portfolio-level translated totals that require full translation are null.

If market pricing is incomplete:

- realized P/L can still be translated if FX exists;
- unrealized and total P/L remain incomplete.

## FX API
Create:

`POST /api/v1/fx-rates`

Latest exact-direction quote:

`GET /api/v1/fx-rates/latest?baseCurrencyId=...&quoteCurrencyId=...`

## Migration
This slice changes persistence and requires an EF Core migration.

Do not hand-author the migration.

Generate it after the solution builds successfully, inspect generated SQL for unexpected DROP/ALTER operations, then apply it.

Expected schema changes:

- nullable `Portfolios.BaseCurrencyId`
- FK `Portfolios.BaseCurrencyId -> Currencies.Id`
- index `IX_Portfolios_BaseCurrencyId`
- new `FxRates` table
- two Currency FKs
- unique pair/date index
- latest-rate lookup index

## Deferred
- provider/source metadata for FX quotes
- bid/ask/mid price semantics
- official vs market FX-source policy
- historical/as-of portfolio P/L translation
- portfolio NAV translation
- cash-balance translation
- FX gains/losses
- base-currency change audit history
- rate triangulation through a third currency
