---
id: DOC-0025
language: English + Persian
owner: Architecture / Portfolio
status: Implemented
title: Historical Portfolio Performance Foundation
version: 1.0.0
---

# Historical Portfolio Performance Foundation
# مبنای عملکرد تاریخی پرتفوی

## 1. Purpose / هدف

EN: Establish the accounting boundary required before implementing TWR, XIRR, or any other return methodology.

FA: مرز حسابداری لازم پیش از پیاده‌سازی TWR، XIRR یا هر روش بازده دیگر ایجاد می‌شود.

This slice intentionally computes an investment profit/loss amount but does **not** publish a percentage return.

## 2. Endpoint

`GET /api/v1/portfolios/{portfolioId}/performance?from=<DateTimeOffset>&to=<DateTimeOffset>`

Requirement:

`From < To`

## 3. Boundary NAV / NAV مرزی

Beginning NAV is reconstructed at `From`.

Ending NAV is reconstructed at `To`.

Both use the existing historical base-currency NAV projection, therefore they inherit:

- historical transaction cutoff;
- historical cash cutoff;
- historical market-price cutoff;
- historical FX cutoff;
- missing-price and missing-FX completeness semantics.

## 4. External cash-flow classification / طبقه‌بندی جریان خارجی

Only these immutable cash-ledger types are external capital movements:

- `Deposit`
- `Withdrawal`

The following are **not** external flows:

- `BuySettlement`
- `SellSettlement`
- `Fee`
- `Tax`
- `Dividend`
- `Interest`
- `OtherCredit`
- `OtherDebit`

Those items remain part of portfolio investment/economic results unless a later explicit policy reclassifies them.

## 5. Period boundary / مرز دوره

External flows are selected in:

`(From, To]`

The lower bound is deliberately exclusive because a transaction occurring exactly at `From` is already included in beginning NAV.

The upper bound is inclusive because ending NAV is inclusive at `To`.

This prevents double counting.

## 6. Signed external flow / جریان خارجی علامت‌دار

Deposit:

`SignedSourceAmount = +Amount`

Withdrawal:

`SignedSourceAmount = -Amount`

Net external flow is the sum of signed translated flows.

## 7. Historical FX for each flow / FX تاریخی هر جریان

Each external flow is translated independently at its own occurrence date.

Eligible FX:

`FxRate.RateDate <= DateOnly(OccurredOn)`

Selection:

1. `RateDate DESC`
2. `CreatedOn DESC`

Direct and inverse pair rules are identical to historical NAV translation.

A later FX rate may affect ending NAV but may not retroactively reprice an earlier external capital contribution.

That difference correctly appears in investment profit/loss as currency movement.

## 8. Investment profit/loss / سود و زیان سرمایه‌گذاری

When both boundary NAVs are complete and every external flow can be translated:

`InvestmentProfitLossBase = EndingNAVBase - BeginningNAVBase - NetExternalFlowBase`

This is an amount, not a return percentage.

## 9. Why no percentage yet? / چرا هنوز درصد بازده نداریم؟

A single percentage requires a policy for the timing and weighting of external flows.

Possible methodologies include:

- Time-Weighted Return (TWR)
- Modified Dietz
- Money-Weighted Return / IRR / XIRR

This foundation deliberately avoids selecting one prematurely.

## 10. Completeness / کامل‌بودن

`IsBeginningNavComplete` requires complete historical beginning NAV.

`IsEndingNavComplete` requires complete historical ending NAV.

`AreExternalFlowsFullyTranslated` requires historical FX for every non-base external flow.

`IsComplete` requires all three.

If incomplete:

- source external flows remain visible;
- missing FX is not zero;
- `NetExternalFlowBase` is null when any external flow lacks FX;
- `InvestmentProfitLossBase` is null.

## 11. Base-currency policy / سیاست ارز پایه

The current configured portfolio base currency is used for both boundary NAV projections and the period result.

Historical base-currency configuration audit is deferred because base-currency changes are not currently versioned.

## 12. Persistence / ماندگاری

No schema change.
No migration.
No performance snapshot is persisted.

## 13. Deferred / موارد موکول‌شده

- reusable FX conversion policy/service;
- external-flow classification policy overrides;
- base-currency change history;
- Modified Dietz;
- TWR subperiod partitioning;
- XIRR / money-weighted return;
- benchmark comparison;
- annualization;
- stale-price policy;
- corporate actions;
- portfolio-performance snapshots.

## 14. Safety / ایمنی

Integration-test database safety guards are not modified.
