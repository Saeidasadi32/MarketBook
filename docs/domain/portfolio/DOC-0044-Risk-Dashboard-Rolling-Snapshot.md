---
id: DOC-0044
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Risk Dashboard Rolling Snapshot
version: 1.0.0
---

# Risk Dashboard Rolling Snapshot
# Snapshot ریسک Rolling داشبورد

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/risk/summary/rolling?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly&windowPeriods=30&confidenceLevel=0.95&riskFreeRateAnnual=0&minimumAcceptableReturnAnnual=0`

## Purpose

DOC-0044 adds a compact latest-point rolling-risk view suitable for dashboard
cards and status panels.

It does not return complete rolling series.

## Source projections

The endpoint composes:

- DOC-0036 Rolling Risk Analytics
- DOC-0040 Rolling Currency Amount Historical VaR + CVaR

No rolling algorithm is recalculated.

## Latest-point policy

For each composed source:

- source ordering remains authoritative
- the last point is the latest full-window point
- `PointCount` preserves the total source point count
- only that latest point is projected into the dashboard response

## Rolling Risk section

The latest DOC-0036 point exposes:

- WindowFrom
- WindowTo
- ObservationCount
- MeanPeriodicReturn
- AnnualizedVolatility
- AnnualizedDownsideDeviation
- SharpeRatio
- SortinoRatio

## Rolling Value-at-Risk section

The latest DOC-0040 point exposes:

- WindowFrom
- WindowTo
- ObservationCount
- TailObservationCount
- IsCalculable
- NetAssetValueBase
- ValueAtRiskReturn
- ConditionalValueAtRiskReturn
- ValueAtRiskAmountBase
- ConditionalValueAtRiskAmountBase

## No-full-window semantics

When a source is complete but there are not enough observations to form a full
rolling window:

- source `IsComplete` remains true
- `HasPoint = false`
- `PointCount = 0`
- latest-point fields are null
- no synthetic point is created

This distinction is important: absence of a full rolling window is not an
incomplete-data error.

## Completeness

Top-level `IsComplete` is true only when both DOC-0036 and DOC-0040 are
complete.

`HasPoint` is independent from completeness.

A complete response may have no point yet.

## Configuration forwarding

The endpoint forwards the same request-scoped settings to the appropriate
sources:

- interval
- windowPeriods
- confidenceLevel
- riskFreeRateAnnual
- minimumAcceptableReturnAnnual

## Base-currency consistency

DOC-0036 and DOC-0040 must resolve the same base currency.

Mismatch error:

`Portfolio.RollingRiskSummary.BaseCurrencyMismatch`

## Response-size policy

DOC-0044 intentionally excludes complete rolling point arrays.

Consumers needing trend charts should use the specialized DOC-0036 and DOC-0040
endpoints.

## Persistence

No schema change.
No migration.

## Safety

Integration-test database safety guards are not modified.

## Deferred

- latest rolling drawdown velocity
- latest rolling beta / tracking error
- benchmark-relative rolling risk
- risk-limit states
- dashboard cache/materialization
- multi-window snapshots such as 20/60/120 periods
