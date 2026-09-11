---
id: DOC-0041
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Monetary Drawdown Analytics
version: 1.0.0
---

# Monetary Drawdown Analytics
# تحلیل افت سرمایه مبلغی

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/drawdown/amount?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly`

Default:

- `interval = Daily`

## Purpose

DOC-0031 measures drawdown from the cash-flow-neutral cumulative TWR wealth
index. DOC-0041 expresses that same drawdown in the portfolio base currency
without reintroducing external capital-flow distortion.

## Composition

DOC-0041 composes:

- DOC-0031 Portfolio Drawdown Analytics
- DOC-0024 Historical Base-Currency NAV + Historical FX Cutoff

It does not recalculate TWR, running peaks, drawdown ratios, historical NAV, or
historical FX conversion.

## Monetary conversion

For a point:

- `D` = drawdown ratio, with `-1 < D <= 0`
- `NAV` = complete historical NAV in base currency at the same timestamp
- `R = 1 + D`

Peak-equivalent NAV at the point's current capital scale:

`EquivalentPeakNAV = NAV / R`

Monetary drawdown:

`DrawdownAmountBase = EquivalentPeakNAV - NAV`

Equivalent:

`DrawdownAmountBase = NAV * (-D) / (1 + D)`

Example:

- relative drawdown = -10%
- current historical NAV = 990

Then:

- equivalent peak NAV = `990 / 0.90 = 1100`
- monetary drawdown = `1100 - 990 = 110`

Using `990 × 10% = 99` would understate the peak-to-trough loss.

## Cash-flow neutrality

Deposit and Withdrawal remain neutral because DOC-0031 supplies the drawdown
ratio from the TWR wealth index.

Historical NAV only supplies the capital scale used to express the already
cash-flow-neutral drawdown as currency.

External flows alone therefore do not create monetary drawdown.

## Point rules

A monetary point requires:

- DOC-0031 point is calculable
- drawdown is available
- DOC-0024 historical NAV is complete
- base currency matches
- `1 + Drawdown > 0`
- historical NAV is non-negative

No amount is fabricated when these requirements fail.

## Summary fields

`CurrentDrawdownAmountBase` is the amount at exact `To`.

`MaximumDrawdownAmountBase` is the amount at the trough belonging to
DOC-0031 `MaximumDrawdown`.

It is intentionally not a separately searched "largest monetary loss" metric.

## Persistence

No schema change.
No migration.

## Deferred

- largest monetary drawdown independent of maximum percentage drawdown
- monetary drawdown episodes
- recovery amount analytics
- underwater capital curve
- drawdown attribution
- batch historical NAV optimization

## Safety

Integration-test database safety guards are not modified.
