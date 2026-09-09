---
id: DOC-0021
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Portfolio NAV by Currency
version: 1.0.0
---

# Portfolio NAV by Currency / NAV پرتفوی به تفکیک ارز

## 1. Purpose / هدف

**EN:** This slice projects current portfolio Net Asset Value independently for each currency. It combines the immutable cash ledger with the current market value of open positions and deliberately avoids direct aggregation across different currencies.

**FA:** این Slice ارزش خالص دارایی جاری پرتفوی را برای هر ارز به‌صورت مستقل محاسبه می‌کند. محاسبه از دفتر نقدی تغییرناپذیر و ارزش بازار جاری موقعیت‌های باز استفاده می‌کند و عمداً از جمع مستقیم ارزهای مختلف خودداری می‌کند.

## 2. API

`GET /api/v1/portfolios/{portfolioId}/nav`

Each currency row exposes:

- `CurrencyId`
- `CashBalance`
- `PricedMarketValue`
- `PricedNetAssetValue`
- `IsComplete`
- `NetAssetValue`
- `PricedPositionCount`
- `UnpricedPositionCount`

## 3. Formulas / فرمول‌ها

For each currency independently:

`CashBalance = sum(PortfolioCashTransaction.SignedAmount)`

This includes deposits, withdrawals, fees, taxes, dividends, interest, other cash movements, and automatic Buy/Sell settlements already recorded in the cash ledger.

`PricedMarketValue = sum(MarketValue of open positions with IsPriced = true)`

`PricedNetAssetValue = CashBalance + PricedMarketValue`

`IsComplete = UnpricedPositionCount == 0`

`NetAssetValue = IsComplete ? PricedNetAssetValue : null`

## 4. Missing market prices / نبود قیمت بازار

**EN:** Missing market data is not treated as zero. The open position remains visible through the valuation projection. Its currency row becomes incomplete, `NetAssetValue` becomes `null`, while `PricedNetAssetValue` remains available as an explicitly partial measure.

**FA:** نبود قیمت بازار به معنی صفر در نظر گرفته نمی‌شود. موقعیت باز در Projection ارزش‌گذاری باقی می‌ماند. ردیف ارز مربوطه ناقص می‌شود، `NetAssetValue` برابر `null` خواهد بود، ولی `PricedNetAssetValue` به‌عنوان مقدار صریحاً ناقص همچنان قابل مشاهده است.

## 5. Reuse of existing projections / استفاده مجدد از Projectionهای موجود

The NAV handler composes:

1. `GetPortfolioCashBalancesQuery`
2. `GetPortfolioValuationQuery`

Therefore NAV inherits the existing portfolio valuation rules, including weighted-average position reconstruction, latest available `MarketPrice.LastPrice`, mixed-currency cost-basis protection, Listing existence checks, and quote-currency snapshot consistency.

## 6. Currency isolation / استقلال ارزها

No USD + EUR + IRR total is produced in this slice. Every monetary value remains inside its own currency row.

Cross-currency NAV translation is deferred to the next slice and will use:

- `Portfolio.BaseCurrencyId`
- `FxRate`
- the existing FX translation convention.

## 7. Persistence

No table, column, index, or migration is introduced. NAV is a current read projection and is not persisted.

## 8. Deferred / موارد بعدی

- Base-currency NAV translation
- historical/as-of NAV
- FX rate as-of valuation date
- NAV history/time series
- TWR and money-weighted performance
- corporate-action-aware historical NAV
- pricing completeness/data-quality metadata
