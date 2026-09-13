---
id: DOC-0042
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Monetary Drawdown Episodes and Recovery
version: 1.0.0
---

# Monetary Drawdown Episodes and Recovery
# دوره‌های افت و بازیابی مبلغی

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/drawdown/episodes/amount?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly`

## Purpose

DOC-0032 defines episode boundaries, troughs, recovery, duration, and active
drawdown status.

DOC-0041 translates cash-flow-neutral drawdown ratios into historical
base-currency monetary amounts.

DOC-0042 composes those two projections without redefining either algorithm.

## Composition

DOC-0042 composes:

- DOC-0032 Drawdown Duration & Recovery Analytics
- DOC-0041 Monetary Drawdown Analytics

It does not independently recalculate:

- TWR
- running peaks
- episode boundaries
- trough selection
- recovery timestamps
- historical NAV
- FX translation
- monetary drawdown formula

## Episode monetary fields

For every episode, the trough timestamp is resolved against DOC-0041.

The response exposes:

- `TroughNetAssetValueBase`
- `TroughEquivalentPeakNetAssetValueBase`
- `TroughDrawdownAmountBase`

For recovered episodes:

- `RecoveryNetAssetValueBase`
- `RecoveredDrawdownAmountBase`

`RecoveredDrawdownAmountBase` equals the monetary drawdown at the trough,
because recovery means the cash-flow-neutral drawdown has returned to zero.

For an active episode:

- `CurrentDrawdownAmountBase`

is resolved from the exact `To` monetary drawdown point.

## Cash-flow neutrality

The monetary fields remain cash-flow-neutral because:

1. DOC-0032 episode semantics are derived from DOC-0031 TWR drawdown.
2. DOC-0041 monetary amounts scale that drawdown by historical base-currency
   exposure without comparing raw peak NAV to raw trough NAV.

Deposit and Withdrawal therefore do not create artificial episode losses or
recoveries.

## Completeness

The endpoint fails closed.

If DOC-0032 is incomplete:

- `IsComplete = false`
- `EpisodeCount = 0`
- no episode summaries are fabricated

If DOC-0041 is incomplete:

- the same fail-closed result is returned

For a mapped episode:

- trough monetary point must be complete and calculable
- recovered episodes also require the recovery point
- active episodes also require the exact `To` point

Top-level `IsComplete` is true only when every mapped episode is complete.

## Summary semantics

`MaximumDrawdownEpisode` keeps DOC-0032's deepest-relative-drawdown selection.

`LongestDrawdownEpisode` keeps DOC-0032's duration selection.

`ActiveDrawdownEpisode` keeps DOC-0032's active-at-To selection.

DOC-0042 only enriches those already-selected episodes with monetary fields.

## Persistence

No schema change.
No migration.

## Deferred

- largest monetary-loss episode independent of relative maximum drawdown
- recovery velocity
- underwater-capital area
- monetary drawdown attribution by asset / currency / sector
- benchmark-relative monetary episode analytics
- batch optimization for historical NAV lookups

## Safety

Integration-test database safety guards are not modified.
