---
id: DOC-0045
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Portfolio Risk Limits and Breach Evaluation Foundation
version: 1.0.0
---

# Portfolio Risk Limits and Breach Evaluation Foundation
# بنیان حدود ریسک و ارزیابی نقض پرتفوی

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/risk/limits/evaluate`

Core query parameters:

- `from`
- `to`
- `interval=Daily|Weekly`
- `confidenceLevel=0.95`
- `riskFreeRateAnnual=0`
- `minimumAcceptableReturnAnnual=0`

Optional request-scoped limits:

- `maxAnnualizedVolatility`
- `maxValueAtRiskReturn`
- `maxValueAtRiskAmountBase`
- `maxDrawdownLossRatio`
- `maxDrawdownAmountBase`
- `minSharpeRatio`
- `minSortinoRatio`

## Purpose

DOC-0045 moves MarketBook from risk measurement toward risk-management
evaluation.

The foundation is intentionally request-scoped.

No risk-policy persistence is introduced yet.

## Composition

DOC-0045 composes DOC-0043 Portfolio Risk Summary Dashboard.

It does not independently recalculate:

- volatility
- VaR
- VaR amount
- drawdown
- drawdown amount
- Sharpe
- Sortino

DOC-0043 remains the source of truth for actual metrics.

## Rule directions

Maximum rules breach when:

`Actual > Limit`

Minimum rules breach when:

`Actual < Limit`

Equality remains within limit.

## Drawdown sign convention

DOC-0043 maximum drawdown is a non-positive return.

Risk policies are easier to configure as positive loss magnitudes.

Therefore:

`MaximumDrawdownLossRatio = max(-MaximumDrawdown, 0)`

Example:

- source MaximumDrawdown = `-0.15`
- policy MaxDrawdownLossRatio = `0.10`
- evaluated actual = `0.15`
- result = `Breached`

## Per-rule statuses

Every rule reports one of:

- `NotConfigured`
- `NotCalculable`
- `WithinLimit`
- `Breached`

A rule is `NotCalculable` when a limit is configured but DOC-0043 does not
provide a calculable metric.

No metric is fabricated.

## Overall status

Precedence:

1. `Incomplete` when DOC-0043 is incomplete.
2. `NoLimitsConfigured` when no limits were supplied.
3. `Breached` when at least one configured rule breaches.
4. `Indeterminate` when no rule breaches but at least one configured rule is
   not calculable.
5. `WithinLimit` otherwise.

## Why Warning is deferred

This foundation does not invent a warning threshold such as 80% or 90% of a
limit.

A later policy model may support explicit warning bands.

Until then, only configured hard limits are evaluated.

## Audit fields

Each rule includes:

- Code
- Direction
- IsConfigured
- Status
- Limit
- Actual
- BreachAmount

For maximum rules:

`BreachAmount = Actual - Limit`

For minimum rules:

`BreachAmount = Limit - Actual`

`BreachAmount` is populated only when breached.

## Persistence

No schema change.
No migration.

Risk limits are request-scoped and are not persisted.

## Safety

Integration-test database safety guards are not modified.

## Deferred

- persisted portfolio risk policy aggregate
- effective-dated policy versions
- warning bands
- policy ownership / approval workflow
- limit severities
- notifications / alerts
- breach acknowledgements
- breach history and audit trail
- rolling-risk limit evaluation
- benchmark-relative limits
- asset / sector / currency concentration limits
