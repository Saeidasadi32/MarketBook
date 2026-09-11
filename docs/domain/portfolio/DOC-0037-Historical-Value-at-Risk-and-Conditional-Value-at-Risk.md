---
id: DOC-0037
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Historical Value at Risk and Conditional Value at Risk
version: 1.0.0
---

# Historical Value at Risk and Conditional Value at Risk
# VaR و CVaR تاریخی

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/risk/var?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly&confidenceLevel=<decimal>`

Defaults:

- `interval = Daily`
- `confidenceLevel = 0.95`

## Method

Foundation method: Historical Simulation.

DOC-0037 composes DOC-0033 risk statistics and reuses its exact periodic TWR returns.

It does not recalculate:

- NAV
- FX
- TWR
- performance series
- periodic returns

## Confidence level

Valid range:

`0 < confidenceLevel < 1`

Typical values:

- 0.95
- 0.99

Tail probability:

`TailProbability = 1 - ConfidenceLevel`

## Historical quantile convention

Returns are sorted ascending.

For N observations:

`TailCountRank = ceil(TailProbability * N)`

The rank is clamped to `[1, N]`.

Historical quantile return:

`QuantileReturn = SortedReturns[TailCountRank - 1]`

This intentionally uses the nearest-rank empirical tail quantile.

No interpolation is performed in this foundation.

## VaR sign convention

VaR is reported as a positive loss ratio:

`VaR = max(-QuantileReturn, 0)`

Examples:

- quantile return = -0.05 => VaR = 0.05
- quantile return = +0.01 => VaR = 0

This avoids reporting negative loss.

## CVaR / Expected Shortfall

Tail observations include every historical return at or below the VaR quantile:

`Tail = { R_i | R_i <= QuantileReturn }`

`AverageTailReturn = average(Tail)`

`CVaR = max(-AverageTailReturn, 0)`

Ties at the VaR threshold are intentionally included for deterministic auditability.

## Minimum data

- incomplete DOC-0033 source => `IsComplete=false`, `IsCalculable=false`
- complete but fewer than 2 periodic returns => `IsComplete=true`, `IsCalculable=false`
- otherwise calculable

No fabricated values.

## Audit fields

Response includes:

- ObservationCount
- ConfidenceLevel
- TailProbability
- HistoricalQuantileReturn
- ValueAtRiskReturn
- ConditionalValueAtRiskReturn
- TailObservationCount
- SortedReturns

## Persistence

No schema change.
No migration.
VaR and CVaR are request-scoped analytics and are not persisted.

## Deferred

- parametric/variance-covariance VaR
- Monte Carlo VaR
- horizon scaling
- currency amount VaR using current NAV
- component / marginal / incremental VaR
- benchmark-relative VaR
- rolling VaR/CVaR
- trading-calendar-aware horizon mapping
- weighted historical simulation
- decay factors
- EVT / tail fitting

## Safety

Integration-test database safety guards are not modified.
