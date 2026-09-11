---
id: DOC-0028
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Performance Comparison Layer — TWR vs XIRR
version: 1.0.0
---

# Performance Comparison Layer — TWR vs XIRR
# لایه مقایسه عملکرد — TWR در برابر XIRR

## 1. Purpose / هدف

EN: Provide one reporting endpoint that presents time-weighted return and money-weighted return side by side without duplicating either methodology.

FA: یک Endpoint گزارش‌گیری فراهم می‌شود که بازده زمان‌وزن و بازده پول‌وزن را بدون تکرار منطق هیچ‌یک، کنار هم نمایش دهد.

## 2. Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/comparison?from=<DateTimeOffset>&to=<DateTimeOffset>`

Requirement:

`From < To`

## 3. Composition policy / سیاست ترکیب

The handler delegates to:

- `GetPortfolioTimeWeightedReturnQuery`
- `GetPortfolioMoneyWeightedReturnQuery`

It does not recalculate:

- historical NAV;
- FX conversion;
- external-flow classification;
- TWR segmentation;
- XIRR root solving.

This keeps the source of truth in the existing feature slices.

## 4. Interpretation / تفسیر

### TWR

TWR neutralizes the timing and size of external Deposit/Withdrawal events.

It is the preferred measure for evaluating the investment process or manager performance independently from investor capital-flow timing.

### XIRR / MWR

XIRR is intentionally sensitive to the size and timing of investor contributions and withdrawals.

It represents the investor's realized experience of capital timing.

## 5. Comparison spread / اختلاف مقایسه

When both measures are available:

`MoneyWeightedMinusTimeWeighted = XIRR - TWR`

A positive spread means the investor's cash-flow timing improved the money-weighted outcome relative to the time-weighted result.

A negative spread means cash-flow timing reduced it.

This spread is descriptive; it is not itself a standalone performance methodology.

## 6. Completeness flags / پرچم‌های کامل‌بودن

`IsDataComplete` is true only when both composed projections have complete historical inputs.

`AreBothReturnsAvailable` is true only when:

- TWR is calculable and present; and
- XIRR has a solved supported root and is present.

The response preserves the individual completeness/solver flags so clients do not need to infer why a return is missing.

## 7. Base currency consistency / سازگاری ارز پایه

Both projections must resolve the same portfolio base currency.

A mismatch returns:

`PortfolioPerformanceComparison.BaseCurrencyMismatch`

This is a defensive consistency guard.

## 8. Persistence / ماندگاری

No schema change.
No migration.
No comparison snapshot is persisted.

## 9. Deferred / موارد موکول‌شده

- period-over-period comparison;
- benchmark-relative performance;
- annualization policy for TWR over arbitrary periods;
- Modified Dietz;
- return attribution;
- dashboard chart DTOs;
- performance snapshot persistence;
- rolling 1M / 3M / 6M / YTD / 1Y / since-inception presets.

## 10. Safety / ایمنی

Integration-test database safety guards are not modified.
