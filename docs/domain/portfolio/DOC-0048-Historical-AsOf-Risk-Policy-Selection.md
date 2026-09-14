# DOC-0048 — Historical / As-Of Portfolio Risk Policy Selection

## Status

Implemented as an additive extension of DOC-0046 and DOC-0047.

## Goal / هدف

EN: Risk-limit evaluation must use the persisted policy that was actually effective at the historical evaluation instant, rather than always using today's Active policy.

FA: ارزیابی حدود ریسک باید Policy ذخیره‌شده‌ای را انتخاب کند که واقعاً در لحظه تاریخی ارزیابی معتبر بوده است، نه اینکه همیشه Policy دارای وضعیت Active امروز را استفاده کند.

## As-Of rule

`EvaluatePortfolioRiskLimitsQuery.To` is the persisted-policy **as-of instant**.

Selection uses half-open effective intervals:

`EffectiveFrom <= To && (EffectiveTo == null || To < EffectiveTo)`

Only non-Draft policies participate.

This means:

- an archived policy can still be selected for a historical instant inside its old effective interval;
- the newly activated policy wins exactly at its `EffectiveFrom`;
- a future policy is never back-applied to an earlier evaluation;
- Draft versions never affect evaluation.

## Resolution precedence

Per field, DOC-0047 precedence remains unchanged:

`explicit request value > effective persisted policy value > NotConfigured`

`LimitSource` semantics remain:

- `None`
- `PersistedPolicy`
- `RequestOverride`
- `Mixed`

## Audit metadata

The response adds:

- `PolicyAsOf` — always equal to request `To`;
- `PolicyId` — selected effective persisted policy, when any;
- `PolicyVersion` — selected effective business version, when any.

## Persistence

`IPortfolioRiskPolicyRepository` adds `GetEffectiveAsOfAsync`.

The EF query:

1. filters by `PortfolioId`;
2. excludes `Draft`;
3. applies the half-open effective interval;
4. orders by `EffectiveFrom` descending and then `PolicyVersion` descending;
5. returns the first matching policy.

The ordering makes selection deterministic even if legacy data contains overlapping effective periods. Preventing arbitrary effective-period overlap remains a separate domain-hardening concern.

## Risk metric source

DOC-0043 remains the sole source of risk metrics. DOC-0048 changes only persisted-policy selection; it does not recalculate analytics.

## Database impact

No schema change and no EF migration are required.

## Compatibility

Current evaluations continue to use the current Active policy whenever that policy is effective at `To`.

Historical requests now correctly select archived historical versions.

## Integration coverage

DOC-0048 adds tests for:

1. historical selection of an archived version;
2. exact activation-boundary selection of the new version;
3. prevention of future-policy back-application.

Expected full integration-suite target after this patch: **197 tests**.
