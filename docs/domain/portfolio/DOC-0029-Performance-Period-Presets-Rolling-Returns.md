---
id: DOC-0029
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Performance Period Presets / Rolling Returns
version: 1.0.0
---

# Performance Period Presets / Rolling Returns
# بازه‌های استاندارد عملکرد / Rolling Returns

## 1. Purpose / هدف

EN: Provide dashboard-ready standard periods while preserving the existing TWR/XIRR methodology as the single source of truth.

FA: بازه‌های استاندارد آماده داشبورد ارائه می‌شوند، در حالی که منطق TWR/XIRR موجود منبع حقیقت باقی می‌ماند.

## 2. Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/presets?asOf=<DateTimeOffset>`

`asOf` is intentionally explicit and required.

The endpoint does not depend on the application server clock.

## 3. Presets / بازه‌ها

The response is ordered as:

1. `1M`
2. `3M`
3. `6M`
4. `YTD`
5. `1Y`
6. `SinceInception`

Boundary rules:

- `1M`: `AsOf.AddMonths(-1)`
- `3M`: `AsOf.AddMonths(-3)`
- `6M`: `AsOf.AddMonths(-6)`
- `YTD`: January 1, 00:00:00 using the same UTC offset as `AsOf`
- `1Y`: `AsOf.AddYears(-1)`
- `SinceInception`: current persisted `Portfolio.CreatedOn`

Every period ends at exactly the supplied `AsOf`.

## 4. Composition / ترکیب

Each preset delegates to:

`GetPortfolioPerformanceComparisonQuery`

Therefore this layer does not reimplement:

- historical NAV;
- historical FX;
- external-flow classification;
- TWR segmentation/chaining;
- XIRR cash-flow construction;
- XIRR solving.

## 5. Since Inception semantics / معنای Since Inception

This foundation defines inception as:

`Portfolio.CreatedOn`

This is a portfolio-metadata boundary, not a first-funded-date boundary.

If NAV at inception is zero, existing TWR/XIRR rules may make one or both returns unavailable. The API preserves that state rather than fabricating a return.

A future explicit `PerformanceInceptionOn` or first-funded-date policy can replace this definition.

## 6. YTD offset policy / سیاست offset برای YTD

YTD begins at January 1 midnight in the same offset carried by the supplied `AsOf`.

Exchange-local/business timezone normalization remains a separate future policy.

## 7. Response / پاسخ

Each item exposes:

- resolved `From`
- common `To`
- completeness
- whether both returns are available
- TWR
- XIRR
- XIRR minus TWR

Clients can render `N/A` when an individual return is null.

## 8. Persistence / ماندگاری

No schema change.
No migration.
No preset snapshot is persisted.

## 9. Deferred / موارد موکول‌شده

- explicit first-funded/investable inception date;
- WTD / MTD / QTD presets;
- rolling 3Y / 5Y;
- fiscal-year presets;
- annualized TWR;
- benchmark-relative rolling returns;
- portfolio/exchange timezone normalization;
- persisted performance snapshots;
- chart-series endpoint.

## 10. Safety / ایمنی

Integration-test database safety guards are not modified.
