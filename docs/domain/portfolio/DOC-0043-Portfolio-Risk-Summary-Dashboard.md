---
id: DOC-0043
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Portfolio Risk Summary Dashboard
version: 1.0.0
---

# Portfolio Risk Summary Dashboard
# داشبورد خلاصه ریسک پرتفوی

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/risk/summary?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly&confidenceLevel=0.95&riskFreeRateAnnual=0&minimumAcceptableReturnAnnual=0`

## Purpose

DOC-0043 provides a compact management-oriented risk snapshot without creating
a new risk formula.

## Source projections

The endpoint composes:

- DOC-0033 Volatility + Downside Deviation
- DOC-0035 Configurable Sharpe + Sortino
- DOC-0039 Currency Amount Historical VaR + CVaR
- DOC-0041 Monetary Drawdown
- DOC-0042 Monetary Drawdown Episodes & Recovery

## Dashboard sections

`RiskStatistics`:
- observation count
- annualization periods
- mean periodic return
- annualized volatility
- annualized downside deviation

`RiskRatios`:
- risk-free rate
- minimum acceptable return
- Sharpe
- Sortino

`ValueAtRisk`:
- confidence level
- observation / tail counts
- NAV at To
- VaR / CVaR ratios
- VaR / CVaR base-currency amounts

`Drawdown`:
- current relative and monetary drawdown
- maximum relative drawdown
- monetary amount at maximum-relative-drawdown trough
- peak / trough timestamps

`DrawdownEpisodes`:
- episode count
- active flag
- deepest episode monetary trough loss
- deepest episode duration
- longest episode duration
- active episode current monetary loss

## Composition rules

No risk algorithm is recalculated in DOC-0043.

Every number remains owned by its source projection.

## Configuration forwarding

The endpoint forwards:

- `interval`
- `confidenceLevel`
- `riskFreeRateAnnual`
- `minimumAcceptableReturnAnnual`

## Completeness

Each section preserves its source completeness/calculability semantics.

Top-level `IsComplete` is true only when all source projections are complete.

No missing analytics are fabricated.

## Base-currency consistency

All source projections must resolve the same base currency.

Mismatch error:

`Portfolio.RiskSummary.BaseCurrencyMismatch`

## Response-size policy

The dashboard excludes large drill-down collections:

- periodic returns
- drawdown points
- full episode arrays
- sorted VaR returns

Specialized endpoints remain available for drill-down.

## Persistence

No schema change.
No migration.

## Safety

Integration-test database safety guards are not modified.

## Deferred

- TWR / XIRR headline
- latest rolling-risk snapshot
- latest rolling-VaR snapshot
- benchmark-relative risk
- risk attribution
- risk limits / alerts
- caching or materialization if composition cost becomes material
