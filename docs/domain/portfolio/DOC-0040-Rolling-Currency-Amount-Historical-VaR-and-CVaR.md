---
id: DOC-0040
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Rolling Currency Amount Historical VaR and CVaR
version: 1.0.0
---

# Rolling Currency Amount Historical VaR and CVaR
# مبلغ VaR و CVaR تاریخی Rolling در ارز پایه

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/risk/var/rolling/amount?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly&windowPeriods=<int>&confidenceLevel=<decimal>`

Defaults:

- `interval = Daily`
- `windowPeriods = 30`
- `confidenceLevel = 0.95`

## Purpose

DOC-0038 produces rolling historical VaR/CVaR loss ratios.

DOC-0040 converts every rolling point into base-currency monetary exposure using
the portfolio's historical translated NAV at that point's own `WindowTo`.

This is intentionally different from multiplying every point by the final NAV
of the entire requested range.

Each point answers:

> Given the historical return distribution in this rolling window and the
> actual portfolio exposure at the end of this same window, what are the
> monetary VaR and CVaR amounts?

## Composition

DOC-0040 composes:

- DOC-0038 Rolling Historical VaR + CVaR
- DOC-0024 Historical Base-Currency NAV + Historical FX Cutoff

It does not recalculate:

- TWR
- periodic returns
- rolling windows
- historical quantiles
- VaR/CVaR tails
- historical position valuation
- FX translation rules

## Point formulas

For each rolling point with complete and strictly-positive historical NAV:

`VaRAmountBase = NetAssetValueBase(WindowTo) * ValueAtRiskReturn`

`CVaRAmountBase = NetAssetValueBase(WindowTo) * ConditionalValueAtRiskReturn`

The VaR/CVaR ratios inherit the DOC-0037/DOC-0038 positive-loss convention.

## Point completeness

A point is complete when historical translated NAV at its exact `WindowTo`
is complete.

A point is calculable when:

- the point is complete
- `NetAssetValueBase > 0`

Otherwise its monetary amount fields are null.

No fabricated NAV or risk amounts.

## Top-level completeness

- if DOC-0038 is incomplete: `IsComplete=false`, zero points
- if DOC-0038 is complete with zero full windows: `IsComplete=true`, zero points
- if rolling points exist: top-level `IsComplete` is true only when every
  point NAV is complete

`CalculablePointCount` reports how many returned points have monetary amounts.

## Currency consistency

Every historical NAV projection must resolve the same base currency as the
rolling risk projection.

A mismatch fails the request rather than mixing currencies.

## Persistence

No schema change.
No migration.

DOC-0040 is request-scoped analytics.

## Deferred

- batch NAV retrieval optimization
- rolling liquidity-adjusted VaR
- rolling stressed VaR
- rolling component / marginal / incremental VaR
- horizon scaling
- expected shortfall attribution
- benchmark-relative rolling monetary risk

## Safety

Integration-test database safety guards are not modified.
