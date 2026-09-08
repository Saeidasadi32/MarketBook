# DOC-0013 — Portfolio Persistence Foundation

## Purpose

This slice makes `Portfolio` a persisted aggregate owned by an `Investor`.

## Ownership

- `Portfolio.InvestorId` is required.
- The database enforces `Portfolio -> Investor` with a foreign key.
- Delete behavior is `Restrict` to preserve ownership history.
- A portfolio can be created only for an active investor.
- A deactivated portfolio cannot be reactivated while its investor is inactive.

## Identity and lifecycle

A portfolio has:

- `PortfolioId` (ULID)
- `InvestorId`
- `PortfolioName`
- `CreatedOn`
- `IsActive`

Supported lifecycle operations:

- Create
- Get by ID
- Get paged list
- Filter by Investor
- Rename
- Activate
- Deactivate

There is intentionally no delete endpoint.

## Name uniqueness

Portfolio names are unique within one investor:

`(InvestorId, Name)`

Different investors may use the same portfolio name.

## Aggregate boundary

`Position` and `PortfolioEvent` remain part of the existing domain aggregate model, but they are deliberately not persisted in this slice.

The EF configuration explicitly ignores:

- `Portfolio.Positions`
- `Portfolio.Events`

This prevents accidental schema coupling before transaction/position accounting rules are finalized.

A later slice will reconcile persistence for portfolio transactions and positions.

## API

Base route:

`/api/v1/portfolios`

Endpoints:

- `POST /api/v1/portfolios`
- `PUT /api/v1/portfolios/{id}`
- `PATCH /api/v1/portfolios/{id}/activate`
- `PATCH /api/v1/portfolios/{id}/deactivate`
- `GET /api/v1/portfolios/{id}`
- `GET /api/v1/portfolios?investorId=...&page=...&pageSize=...`

## Migration safety

Do not ship a hand-written migration.

After the patch builds successfully:

1. Generate `AddPortfolio` with EF Core.
2. Verify it sorts after `20260908045957_AddInvestor`.
3. Generate SQL from `20260908045957_AddInvestor` to the new migration.
4. Confirm the SQL contains only expected `Portfolios` table/index/FK changes.
5. Only then run `database update`.
