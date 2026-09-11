---
id: DOC-0036
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Rolling Risk Analytics
version: 1.0.0
---

# Rolling Risk Analytics
# تحلیل ریسک Rolling

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/risk/rolling?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly&windowPeriods=<int>&riskFreeRateAnnual=<decimal>&minimumAcceptableReturnAnnual=<decimal>`

Defaults:

- `interval = Daily`
- `windowPeriods = 30`
- `riskFreeRateAnnual = 0`
- `minimumAcceptableReturnAnnual = 0`

## Composition

DOC-0036 composes DOC-0033 risk statistics and reuses its exact periodic TWR returns.

It does not recalculate:

- NAV
- FX
- TWR
- Performance Series
- periodic returns

## Window semantics

`WindowPeriods` is the number of periodic-return observations in each rolling window.

Examples:

- Daily + 30 => 30 daily return observations
- Weekly + 12 => 12 weekly return observations

The first rolling point is emitted only after a full window is available.

No partial warm-up windows are emitted.

Valid range:

- minimum = 2
- maximum = 1000

## Metrics per rolling window

Each full window calculates:

- arithmetic MeanPeriodicReturn
- sample PeriodicVolatility using `N - 1`
- AnnualizedVolatility
- downside deviation relative to configured MAR using all `N`
- AnnualizedDownsideDeviation
- SharpeRatio using configured risk-free rate
- SortinoRatio using configured MAR

## Rate convention

The DOC-0035 arithmetic annual convention is reused:

`RiskFreePeriodic = RiskFreeAnnual / P`

`MARPeriodic = MARAnnual / P`

Daily => P = 365
Weekly => P = 52

## Sharpe

`Sharpe = (Mean - RiskFreePeriodic) / Volatility * sqrt(P)`

When volatility is zero:

- SharpeRatio = null

## Sortino

For each return:

`D_i = min(R_i - MARPeriodic, 0)`

`DownsideDeviation = sqrt(Σ(D_i^2) / N)`

`Sortino = (Mean - MARPeriodic) / DownsideDeviation * sqrt(P)`

When downside deviation is zero:

- SortinoRatio = null

## Completeness

If DOC-0033 source data is incomplete:

- IsComplete = false
- PointCount = 0
- Points = []

If source data is complete but fewer than WindowPeriods returns exist:

- IsComplete = true
- PointCount = 0
- Points = []

No partial statistic is fabricated.

## Persistence

No schema change.
No migration.
Rolling metrics are request-scoped and are not persisted.

## Deferred

- trading-calendar-aware rolling windows
- date-duration windows such as trailing 30 calendar days
- rolling beta / alpha
- benchmark-relative rolling analytics
- persisted risk snapshots
- percentile bands
- VaR / CVaR

## Safety

Integration-test database safety guards are not modified.
