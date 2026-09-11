---
id: DOC-0032
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Drawdown Duration and Recovery Analytics
version: 1.0.0
---

# Drawdown Duration and Recovery Analytics
# تحلیل مدت افت و بازیابی

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/drawdown/episodes?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly`

## Composition rule

This feature composes `GetPortfolioDrawdownQuery`.

It does not recalculate:

- NAV
- FX translation
- TWR
- wealth index
- point drawdown

## Episode definition

A drawdown episode begins at the running peak immediately preceding the first sampled point whose drawdown is below zero.

The episode trough is the sampled point with the most negative drawdown.

An episode recovers at the first sampled point whose drawdown returns to zero or above.

If no recovery exists before exact `To`, the episode is ongoing.

## Durations

For a recovered episode:

- `PeakToTroughDays = Trough - Peak`
- `RecoveryDays = Recovery - Trough`
- `TotalDurationDays = Recovery - Peak`

For an ongoing episode:

- `RecoveryTimestamp = null`
- `RecoveryDays = null`
- `TotalDurationDays = To - Peak`

Durations use exact elapsed time expressed as decimal days.

## Summary

The response exposes:

- EpisodeCount
- HasActiveDrawdown
- MaximumDrawdownEpisode
- LongestDrawdownEpisode
- ActiveDrawdownEpisode
- ordered Episodes

Maximum-drawdown ties and duration ties resolve deterministically by earlier peak timestamp.

## Cash-flow neutrality

Because DOC-0031 drawdown is based on the cumulative TWR wealth index, Deposit and Withdrawal do not create drawdown episodes by themselves.

## Incomplete source data

Duration and recovery require continuity.

If the underlying drawdown series is incomplete, this foundation does not infer an episode across the gap:

- `IsComplete = false`
- episode summaries are null
- `Episodes = []`

This is intentionally conservative.

## Sampling caveat

Peak, trough, and recovery timestamps are observed sampled timestamps, not continuous-time extrema.

With `Daily`, a recovery is known at the first daily sample at/above the peak.
With `Weekly`, recovery precision is weekly.

## Persistence

No schema change.
No migration.
No episode snapshots are persisted.

## Deferred

- underwater-day count excluding non-trading days
- trading-calendar-aware duration
- rolling drawdown windows
- benchmark-relative drawdown
- recovery confidence on sparse intervals
- materialized risk snapshots
- volatility
- downside deviation
- Sharpe ratio
- Sortino ratio

## Safety

Integration-test database safety guards are not modified.
