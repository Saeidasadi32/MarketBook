---
id: DOC-0031
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Portfolio Drawdown Analytics
version: 1.1.0
---

# Portfolio Drawdown Analytics
# تحلیل افت سرمایه پرتفوی

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/drawdown?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly`

## Core policy

Drawdown is calculated from the cumulative **TWR wealth index**, not raw NAV.

This is required because raw NAV changes when external capital is deposited or withdrawn.
Such capital flows are not investment gains or losses and must not create artificial drawdown.

## Formula

`WealthIndex(t) = 1 + CumulativeTWR(t)`

`RunningPeak(t) = max(WealthIndex(s))`

`Drawdown(t) = WealthIndex(t) / RunningPeak(t) - 1`

At `From`, cumulative TWR is zero when calculable, therefore the wealth index begins at `1`.

Example:

- wealth index peak = 1.20
- later wealth index = 0.96

`Drawdown = 0.96 / 1.20 - 1 = -0.20`

Maximum drawdown is therefore `-20%`.

## External cash flows

Deposit and Withdrawal are neutralized by the existing TWR projection.

Consequently they do not create drawdown by themselves.

## Response

The response exposes:

- PeakWealthIndex
- PeakTimestamp
- CurrentDrawdown
- MaximumDrawdown
- MaximumDrawdownPeakTimestamp
- MaximumDrawdownTroughTimestamp
- complete ordered drawdown point series

Each point includes:

- cumulative TWR
- wealth index
- running peak wealth index
- running peak timestamp
- drawdown

## Incomplete data

If cumulative TWR is unavailable at a sampled point:

- that point remains visible;
- Drawdown is null;
- overall `IsComplete` becomes false;
- no fabricated value is produced.

## Persistence and safety

No schema change.
No migration.
Integration-test database safety guards are not modified.
