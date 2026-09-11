---
id: DOC-0039
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Currency Amount Historical Value at Risk and Conditional Value at Risk
version: 1.0.0
---

# Currency Amount Historical Value at Risk and Conditional Value at Risk
# مبلغ VaR و CVaR تاریخی در ارز پایه

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance/risk/var/amount?from=<DateTimeOffset>&to=<DateTimeOffset>&interval=Daily|Weekly&confidenceLevel=<decimal>`

Defaults:

- `interval = Daily`
- `confidenceLevel = 0.95`

## Purpose

DOC-0037 expresses historical VaR and CVaR as positive loss ratios.

DOC-0039 translates those ratios into monetary amounts using the portfolio's
complete translated NAV at the exact historical `To` instant.

This makes a historical query genuinely historical:

- the return distribution is bounded by `From` and `To`
- the exposure amount is NAV at `To`
- FX translation uses the existing historical FX cutoff contract from DOC-0024
- current NAV is never substituted for historical NAV

## Composition

DOC-0039 composes:

- DOC-0037 Historical VaR + CVaR
- DOC-0024 Historical Base-Currency NAV + Historical FX Cutoff

It does not recalculate:

- historical returns
- TWR
- historical quantiles
- VaR/CVaR tail observations
- position valuation
- FX conversion

## Amount formulas

When the composed data is complete and the ending NAV is strictly positive:

`VaRAmountBase = NetAssetValueBase(To) * ValueAtRiskReturn`

`CVaRAmountBase = NetAssetValueBase(To) * ConditionalValueAtRiskReturn`

VaR/CVaR ratios already use the DOC-0037 positive-loss convention, therefore
amounts are non-negative.

## Completeness and calculability

`IsComplete` is true only when:

- DOC-0037 return-risk data is complete
- DOC-0024 translated historical NAV is complete

`IsCalculable` additionally requires:

- DOC-0037 VaR/CVaR is calculable
- VaR ratio exists
- CVaR ratio exists
- complete ending NAV exists
- `NetAssetValueBase > 0`

A complete zero or negative NAV is not fabricated into a currency risk amount.
The response remains complete but not calculable and amount fields are null.

If the risk projection and NAV projection resolve different base currencies,
the request fails rather than silently mixing currencies.

## Response

- PortfolioId
- BaseCurrencyId
- From
- To
- Interval
- ConfidenceLevel
- IsComplete
- IsCalculable
- ObservationCount
- TailObservationCount
- NetAssetValueBase
- ValueAtRiskReturn
- ConditionalValueAtRiskReturn
- ValueAtRiskAmountBase
- ConditionalValueAtRiskAmountBase

## Persistence

No schema change.
No migration.

DOC-0039 is a request-scoped analytical projection.

## Deferred

- rolling currency-amount VaR/CVaR
- multi-horizon amount scaling
- position/component VaR
- marginal VaR
- incremental VaR
- liquidity-adjusted VaR
- benchmark-relative VaR
- stressed VaR
- Monte Carlo amount VaR

## Safety

Integration-test database safety guards are not modified.
