---
id: DOC-0027
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Money-Weighted Return / XIRR Foundation
version: 1.0.0
---

# Money-Weighted Return / XIRR Foundation
# مبنای بازده پول‌وزن / XIRR

## Purpose / هدف

This slice measures the investor's annualized return from dated capital flows and terminal portfolio value.

Unlike TWR, MWR/XIRR is intentionally sensitive to the size and timing of external contributions and withdrawals.

## Endpoint

`GET /api/v1/portfolios/{portfolioId}/xirr?from=<DateTimeOffset>&to=<DateTimeOffset>`

Requirement:

`From < To`

## Cash-flow stream / جریان‌های نقدی

Investor perspective is used:

- Beginning NAV at `From` => negative cash flow.
- Deposit => negative cash flow.
- Withdrawal => positive cash flow.
- Ending NAV at `To` => positive terminal cash flow.

Deposit/Withdrawal values reuse the historical base-currency translation from DOC-0025.

Internal portfolio cash events are not investor capital flows:

- BuySettlement
- SellSettlement
- Fee
- Tax
- Dividend
- Interest
- OtherCredit
- OtherDebit

## XIRR equation

The solved annualized rate `r` satisfies:

`sum(CF_i / (1 + r) ^ ((Date_i - Date_0).TotalDays / 365)) = 0`

The implementation uses actual elapsed days and a 365-day annualization denominator.

## Solver policy

A bounded bisection solver is used for determinism and numerical stability.

Search domain:

`-0.9999 <= r <= 1000`

The solver requires a sign change across the supported domain.

If no supported root is bracketed:

- `HasSolution = false`
- `AnnualizedMoneyWeightedReturn = null`

No guessed or synthetic value is returned.

## Completeness

XIRR requires:

- complete beginning historical translated NAV;
- complete ending historical translated NAV;
- complete historical FX translation for every external flow.

Missing price or missing FX never becomes zero.

## Cash-flow sign validation

At least one negative and one positive investor cash flow are required.

Otherwise:

- `HasValidCashFlowSigns = false`
- no solver attempt is made.

## Multiple roots

Cash-flow streams with multiple sign changes can mathematically admit multiple IRR roots.

This foundation intentionally returns only a root that is bracketed across the supported domain. A future policy can add root scanning and ambiguity reporting if needed.

## Persistence

No schema change.
No migration.
No XIRR snapshot is persisted.

## Deferred

- explicit multi-root scanning and ambiguity detection;
- configurable day-count convention;
- Modified Dietz;
- benchmark-relative money-weighted return;
- annual performance snapshots;
- historical base-currency configuration audit;
- stale-price policy;
- corporate actions;
- external-flow classification overrides.

## Safety

Integration-test database safety guards are not modified.
