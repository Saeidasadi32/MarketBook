---
id: DOC-0030
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Performance Chart Series Foundation
version: 1.0.0
---

# Performance Chart Series Foundation
# مبنای سری زمانی نمودار عملکرد

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/series?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly`

## Semantics

- `From < To`
- default interval: `Daily`
- supported intervals: `Daily`, `Weekly`
- exact `From` and exact `To` are always present
- a non-aligned `To` is appended as the last point
- maximum 1000 points per request

Each point reuses historical translated NAV.
Each point after `From` reuses cumulative TWR from `From`.
At `From`, cumulative TWR is zero only when beginning NAV is complete and strictly positive.

Exact Deposit/Withdrawal markers are reused from the historical performance projection.

The series is complete only when all NAV/TWR points are available and all external flows have historical FX translation.

No schema change.
No migration.
No server-clock dependency.
Integration-test database safety guards are untouched.

## Deferred

- Monthly cadence
- automatic cadence selection
- benchmark series
- drawdown series
- rolling volatility
- daily return series
- bulk historical projection optimization
- persisted performance snapshots
- exchange-local session-close normalization
