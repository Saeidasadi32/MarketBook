---
id: DOC-0035
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Configurable Risk-Free Rate and Minimum Acceptable Return
version: 1.0.0
---

# Configurable Risk-Free Rate and Minimum Acceptable Return
# نرخ بدون‌ریسک و حداقل بازده قابل‌قبول قابل‌تنظیم

## Endpoint

Backward-compatible extension of DOC-0034:

`GET /api/v1/portfolios/{portfolioId}/performance/risk-ratios?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly&riskFreeRateAnnual=<decimal>&minimumAcceptableReturnAnnual=<decimal>`

Both new query parameters default to zero.

Existing DOC-0034 requests therefore retain identical behavior.

## Composition

The feature continues to compose DOC-0033 risk statistics.

It reuses:

- periodic TWR returns
- mean periodic return
- sample periodic volatility
- annualization cadence

It does not rebuild NAV, FX, TWR, or periodic returns.

## Annual-to-periodic rate convention

DOC-0035 defines the configured annual rates as arithmetic annual rates.

For `P` annualization periods:

`RiskFreePeriodic = RiskFreeAnnual / P`

`MARPeriodic = MARAnnual / P`

Where:

- Daily => `P = 365`
- Weekly => `P = 52`

This convention is intentionally aligned with the arithmetic-mean Sharpe/Sortino foundation.

## Configurable Sharpe

`MeanExcessRf = MeanPeriodicReturn - RiskFreePeriodic`

`Sharpe = MeanExcessRf / PeriodicVolatility * sqrt(P)`

Volatility remains the sample volatility from DOC-0033.

If periodic volatility is zero, Sharpe is null and `IsSharpeCalculable = false`.

## Configurable Sortino

Sortino requires downside deviation relative to the configured MAR.

For each periodic return:

`D_i = min(R_i - MARPeriodic, 0)`

`DownsideDeviationMAR = sqrt(Σ(D_i^2) / N)`

`MeanExcessMAR = MeanPeriodicReturn - MARPeriodic`

`Sortino = MeanExcessMAR / DownsideDeviationMAR * sqrt(P)`

The denominator uses all `N` observations, consistent with DOC-0033.

DOC-0033 zero-target downside deviation is not reused when MAR is nonzero.

## Audit fields

Response includes:

- RiskFreeRateAnnual
- RiskFreeRatePeriodic
- MinimumAcceptableReturnAnnual
- MinimumAcceptableReturnPeriodic
- MeanPeriodicReturn
- MeanPeriodicExcessReturnOverRiskFree
- MeanPeriodicExcessReturnOverMinimumAcceptableReturn
- PeriodicVolatility
- PeriodicDownsideDeviationRelativeToMinimumAcceptableReturn
- SharpeRatio
- SortinoRatio

## Backward compatibility

When both configured annual rates are zero:

- RiskFreeRatePeriodic = 0
- MARPeriodic = 0
- Sharpe matches DOC-0034
- Sortino matches DOC-0034

## Persistence

No schema change.
No migration.
Configured rates are request-scoped and are not persisted.

## Deferred

- persisted portfolio-level risk policy
- market/yield-curve sourced risk-free rate
- currency-specific risk-free curves
- effective-date rate history
- configurable compounded annual-to-periodic conversion
- rolling Sharpe / Sortino
- benchmark-relative ratios
- trading-calendar-aware annualization

## Safety

Integration-test database safety guards are not modified.
