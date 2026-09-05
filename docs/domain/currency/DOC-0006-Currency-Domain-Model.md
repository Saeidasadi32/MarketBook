# Currency Domain Model

Status date: 2026-09-05

`Currency` represents the quote/valuation monetary unit used by downstream financial models such as Listing.

## Invariants

- `CurrencyId` is a strongly typed ULID identifier.
- `CurrencyCode` is normalized to uppercase and must contain exactly three ASCII letters.
- `Name` is required and trimmed.
- `DecimalPlaces` must be between 0 and 18 inclusive.
- Currency codes are unique at persistence level.
- Activate and Deactivate are idempotent lifecycle operations.

## API surface

Base route: `api/v1/currencies`

- `POST /` — Create
- `GET /{id}` — GetById
- `GET /?page=1&pageSize=20` — paged list
- `PUT /{id}` — Update Code, Name, DecimalPlaces
- `PATCH /{id}/activate` — Activate
- `PATCH /{id}/deactivate` — Deactivate

## Error contract

Application errors use the `Currency.*` namespace, including `InvalidId`, `NotFound`, `InvalidCode`, `InvalidName`, `InvalidDecimalPlaces`, and `DuplicateCode`.
