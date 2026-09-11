---
id: DOC-0033
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Volatility and Downside Deviation Foundation
version: 1.0.0
---

# Volatility and Downside Deviation Foundation
# زیربنای نوسان و انحراف نزولی

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/risk-statistics?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly`

## Composition

The feature composes the existing Performance Chart Series.

It does not recalculate:

- historical NAV
- FX translation
- external-flow neutralization
- cumulative TWR

## Periodic returns

For adjacent cumulative TWR wealth-index points:

`W_i = 1 + CumulativeTWR_i`

`R_i = W_i / W_(i-1) - 1`

This reconstructs cash-flow-neutral periodic investment returns.

## Mean periodic return

Arithmetic mean:

`Mean = ΣR_i / N`

## Volatility

Periodic volatility uses the sample standard deviation:

`Variance_sample = Σ(R_i - Mean)^2 / (N - 1)`

`PeriodicVolatility = sqrt(Variance_sample)`

At least two return observations are required.

## Downside deviation

The minimum acceptable return / target return in this foundation is exactly zero.

For each observation:

`D_i = min(R_i, 0)`

`PeriodicDownsideDeviation = sqrt(Σ(D_i^2) / N)`

The denominator is all N observations, not only negative observations.

This convention is fixed in DOC-0033 and is the intended foundation for Sortino ratio.

## Annualization

Because the current Performance Series cadence is calendar-based:

- Daily => 365 periods/year
- Weekly => 52 periods/year

`AnnualizedVolatility = PeriodicVolatility * sqrt(PeriodsPerYear)`

`AnnualizedDownsideDeviation = PeriodicDownsideDeviation * sqrt(PeriodsPerYear)`

Trading-calendar-aware annualization is deferred.

## Completeness

If the source performance series is incomplete or any wealth index is non-positive:

- `IsComplete = false`
- risk metrics are null
- no fabricated statistic is produced.

If source data is complete but fewer than two periodic returns exist:

- `IsComplete = true`
- `IsCalculable = false`
- risk metrics are null.

## Response auditability

The exact ordered periodic returns used by the calculation are returned.

This allows later Sharpe/Sortino composition without silently changing return construction.

## Persistence

No schema change.
No migration.
No risk-statistic snapshots are persisted.

## Deferred

- trading-calendar-aware annualization
- configurable minimum acceptable return
- risk-free-rate model
- Sharpe ratio
- Sortino ratio
- rolling volatility
- rolling downside deviation
- benchmark-relative volatility
- materialized risk snapshots

## Safety

Integration-test database safety guards are not modified.
