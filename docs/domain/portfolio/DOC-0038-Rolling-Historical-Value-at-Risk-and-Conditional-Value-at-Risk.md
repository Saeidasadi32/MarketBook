---
id: DOC-0038
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Rolling Historical Value at Risk and Conditional Value at Risk
version: 1.0.0
---

# Rolling Historical Value at Risk and Conditional Value at Risk
# VaR و CVaR تاریخی Rolling

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/risk/var/rolling?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly&windowPeriods=<int>&confidenceLevel=<decimal>`

Defaults:

- `interval = Daily`
- `windowPeriods = 30`
- `confidenceLevel = 0.95`

## Composition

DOC-0038 composes:

- DOC-0033 exact periodic TWR returns
- DOC-0037 historical VaR/CVaR calculation convention

It does not recalculate:

- NAV
- FX
- TWR
- performance series
- periodic returns

The DOC-0037 historical calculator is shared by both non-rolling and rolling analytics so quantile and tail semantics cannot drift.

## Window convention

`WindowPeriods` is the number of periodic-return observations in each full rolling window.

Valid range:

`2 <= WindowPeriods <= 1000`

No partial warm-up windows are emitted.

If N periodic returns are complete:

`PointCount = max(N - WindowPeriods + 1, 0)`

The point window timestamps are:

- `WindowFrom = first return.From`
- `WindowTo = last return.To`

## Confidence level

Valid range:

`0 < ConfidenceLevel < 1`

Tail probability:

`TailProbability = 1 - ConfidenceLevel`

## Historical VaR/CVaR convention

For each full rolling window, returns are sorted ascending.

Nearest-rank empirical tail rank:

`TailCountRank = ceil(TailProbability * WindowPeriods)`

Rank is clamped to `[1, WindowPeriods]`.

Historical quantile return:

`QuantileReturn = SortedWindowReturns[TailCountRank - 1]`

VaR positive loss convention:

`VaR = max(-QuantileReturn, 0)`

CVaR tail:

`Tail = { R_i | R_i <= QuantileReturn }`

`CVaR = max(-average(Tail), 0)`

All observations tied at the VaR threshold are included.

No interpolation is performed.

## Completeness

- incomplete DOC-0033 source => `IsComplete=false`, zero points
- complete source with fewer observations than `WindowPeriods` => `IsComplete=true`, zero points
- otherwise one point per full rolling window

No fabricated values.

## Response

Top-level:

- PortfolioId
- BaseCurrencyId
- From
- To
- Interval
- IsComplete
- WindowPeriods
- ConfidenceLevel
- TailProbability
- PointCount
- Points

Each point:

- WindowFrom
- WindowTo
- ObservationCount
- HistoricalQuantileReturn
- ValueAtRiskReturn
- ConditionalValueAtRiskReturn
- TailObservationCount

## Persistence

No schema change.
No migration.
Rolling VaR/CVaR is request-scoped analytics and is not persisted.

## Deferred

- currency-amount rolling VaR/CVaR
- rolling horizon scaling
- weighted historical simulation
- decay factors
- parametric rolling VaR
- Monte Carlo rolling VaR
- component / marginal / incremental rolling VaR
- benchmark-relative rolling VaR
- EVT / tail fitting

## Safety

Integration-test database safety guards are not modified.
