---
id: DOC-0034
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Sharpe and Sortino Foundation
version: 1.0.0
---

# Sharpe and Sortino Foundation
# زیربنای Sharpe و Sortino

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/risk-ratios?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly`

## Composition

This feature composes DOC-0033 `GetPortfolioRiskStatisticsQuery`.

It does not recalculate:

- TWR
- periodic returns
- arithmetic mean return
- volatility
- downside deviation
- annualization cadence

## Foundation policy

To remain exactly consistent with DOC-0033:

- annual risk-free rate = `0`
- annual minimum acceptable return / Sortino target = `0`

Configurable rates are deferred.

## Sharpe ratio

For periodic arithmetic mean return `Mean`,
periodic sample volatility `Volatility`,
and annualization periods `P`:

`Sharpe = (Mean / Volatility) * sqrt(P)`

Because the foundation risk-free rate is zero, mean excess return equals mean return.

Sharpe is calculable only when:

- DOC-0033 is calculable
- Mean is available
- PeriodicVolatility > 0

If volatility is zero, Sharpe is null.
No infinity or fabricated ratio is returned.

## Sortino ratio

For zero-target periodic downside deviation `DownsideDeviation`:

`Sortino = (Mean / DownsideDeviation) * sqrt(P)`

Sortino is calculable only when:

- DOC-0033 is calculable
- Mean is available
- PeriodicDownsideDeviation > 0

If downside deviation is zero, Sortino is null.
This includes all-nonnegative return series.

## Annualization

Annualization is inherited from DOC-0033:

- Daily => 365
- Weekly => 52

No independent cadence policy is introduced here.

## Completeness

`IsComplete` mirrors DOC-0033.

Calculability is exposed separately for Sharpe and Sortino because:

- volatility can be zero while source data is complete
- downside deviation can be zero while source data is complete

## Persistence

No schema change.
No migration.
No ratio snapshots are persisted.

## Deferred

- configurable annual risk-free rate
- configurable minimum acceptable return
- yield-curve-backed risk-free rate
- geometric / compounded excess-return variants
- rolling Sharpe
- rolling Sortino
- benchmark-relative ratios
- trading-calendar-aware annualization
- materialized risk snapshots

## Safety

Integration-test database safety guards are not modified.
