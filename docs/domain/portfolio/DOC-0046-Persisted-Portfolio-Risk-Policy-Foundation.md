---
id: DOC-0046
title: Persisted Portfolio Risk Policy Foundation
status: Implemented
---

# DOC-0046 — Persisted Portfolio Risk Policy Foundation

## Purpose
Persist versioned hard risk limits independently from the Portfolio aggregate.

## Aggregate
`PortfolioRiskPolicy` is a separate aggregate root referencing `PortfolioId`.

`PolicyVersion` is intentionally distinct from `AggregateRoot.Version`, which remains the technical/domain-event version.

## Lifecycle
- first policy: version 1, Active
- revisions: next version, Draft
- activation: previous Active becomes Archived; selected version becomes Active
- archival: selected version becomes Archived

## Limits
Maximum limits must be greater than zero when configured.
Minimum Sharpe/Sortino limits may be negative.

## Endpoints
- POST `/api/v1/portfolios/{id}/risk-policy`
- GET `/api/v1/portfolios/{id}/risk-policy`
- GET `/api/v1/portfolios/{id}/risk-policy/history`
- PUT `/api/v1/portfolios/{id}/risk-policy`
- POST `/api/v1/portfolios/{id}/risk-policy/{version}/activate?effectiveFrom=...`
- DELETE `/api/v1/portfolios/{id}/risk-policy/{version}?effectiveTo=...`

## Database invariants
- unique `(PortfolioId, PolicyVersion)`
- filtered unique active policy per Portfolio
- FK to Portfolio with Restrict delete

## Migration safety
The migration is NOT hand-authored in this patch.
Generate it on the developer machine after a green build, inspect generated SQL for unexpected destructive operations, then run tests.

## Integration-test safety
The database-name safety guard is unchanged.
Reset ordering only deletes `PortfolioRiskPolicy` before deleting its parent `Portfolio`.

## Deferred
DOC-0045 automatic fallback to persisted policy is intentionally deferred to the next integration slice so persistence semantics can be validated first.
Effective-period overlap enforcement beyond the single-active invariant is also deferred until historical-as-of policy selection is introduced.
