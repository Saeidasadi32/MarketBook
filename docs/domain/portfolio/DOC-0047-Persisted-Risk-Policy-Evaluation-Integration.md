---
id: DOC-0047
title: Persisted Risk Policy Evaluation Integration
status: Implemented
---

# DOC-0047 — Persisted Risk Policy Evaluation Integration

## Purpose

Connect DOC-0045 risk-limit evaluation to the active persisted policy introduced by DOC-0046 without breaking existing request-scoped behavior.

## Resolution precedence

Resolution is performed independently for every supported limit:

1. explicit request/query value;
2. corresponding value from the active persisted `PortfolioRiskPolicy`;
3. not configured.

A request value therefore overrides only the matching persisted field. Other missing request fields may still fall back to the active policy.

## Limit-source metadata

The response adds audit metadata:

- `None`: neither request nor active persisted policy supplied a usable limit.
- `PersistedPolicy`: all configured limits came from the active persisted policy.
- `RequestOverride`: configured limits came from request values and no persisted fallback field was used.
- `Mixed`: at least one request override and at least one different persisted fallback field were used.

When an active policy participates in resolution, `PolicyId` and `PolicyVersion` identify it.

## Compatibility

The existing endpoint and query parameters remain unchanged:

`GET /api/v1/portfolios/{id}/performance/risk/limits/evaluate`

Existing callers that send request-scoped limits retain precedence.

If there are no request limits and no active persisted policy limits, DOC-0045 behavior remains:

`OverallStatus = NoLimitsConfigured`

## Risk metric source

DOC-0043 remains the sole source of risk metrics. DOC-0047 does not recalculate volatility, VaR, drawdown, Sharpe, or Sortino.

## Persistence and migration

No database schema change is required. No migration is included.

## Integration-test safety

The integration-test database safety guard is not modified.

## Tests

Four integration scenarios are added:

1. active persisted-policy fallback;
2. request override of persisted value;
3. mixed persisted fallback plus request override;
4. preservation of `NoLimitsConfigured` when no active policy exists.
