# DEV-0001 — Implementation Conventions

These conventions reflect the current working MarketBook slices and should be used for new end-to-end features unless superseded by an ADR.

## C# file header

New or changed C# files use the standard MarketBook header with Project, Platform, Layer, Namespace, copyright, and MIT license lines.

## XML documentation

Classes, constructors, and public methods should use bilingual documentation with explicit `EN:` and `FA:` text. Keep one format consistently; avoid mixing `EN:/FA:` with ad-hoc `<para>` translations in newly changed files.

## Commands and queries

Feature folders use:

- `Features/<Aggregate>/Commands/<Operation>`
- `Features/<Aggregate>/Queries/<Operation>`

MediatR dispatches requests. The repository currently contains both direct `IRequest<T>` usage and generic `ICommand`/`IQuery` abstractions. Do not migrate existing slices opportunistically; choose one standard through a dedicated architecture decision before a broad refactor.

## Results and errors

Handlers return `Result<T>` and use stable error codes such as:

- `<Aggregate>.InvalidId`
- `<Aggregate>.NotFound`
- `<Aggregate>.InvalidName`
- `<Aggregate>.DuplicateCode`

API maps errors through `ApiErrorMapper` rather than returning hand-built error responses from each controller.

## IDs

API/Application boundaries receive string IDs and parse them into strongly typed IDs. Domain code works with strongly typed IDs.

Controller ID responses use the established value rendering pattern:

`result.Value!.Value.ToString()`

## Persistence

- Application owns repository interfaces and `IApplicationDbContext`.
- Infrastructure owns EF Core implementations.
- Read repository methods may use `AsNoTracking()`.
- When a detached aggregate is updated, the repository `Update()` method is used before `SaveChangesAsync()`.

## Pagination

Use `PageRequest` and `PagedResult`.

Current normalization:

- page < 1 => 1
- pageSize < 1 => 20
- pageSize > 100 => 100

Ordering, counting, skipping, and taking must remain server-side.

## Aggregate lifecycle

Prefer explicit `Activate()` / `Deactivate()` methods when historical references matter. Hard delete should be introduced only for a clear business requirement.
