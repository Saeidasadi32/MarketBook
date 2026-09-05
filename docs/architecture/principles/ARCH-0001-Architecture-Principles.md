---
id: ARCH-0001
title: Architecture Principles
version: 1.1.0
status: Accepted
owner: Architecture
---

# Architecture Principles

## 1. Dependency direction

Dependencies point inward:

`Api -> Application -> Domain`

`Infrastructure -> Application + Domain`

Domain must not depend on Application, Infrastructure, API, EF Core, ASP.NET Core, or transport concerns.

## 2. Domain model owns invariants

Aggregate behavior must be expressed through methods such as `Rename`, `Activate`, `Deactivate`, `AssignExchange`, or `ChangeType`, instead of exposing unrestricted public setters.

Application handlers validate cross-aggregate and persistence-dependent rules; aggregate methods protect local business invariants.

## 3. CQRS / Vertical Slice at the Application boundary

Each use case is grouped by feature and operation. Commands mutate state; queries read state. MediatR is the current dispatch mechanism.

New features should follow the established slice structure unless an explicit architecture decision changes it.

## 4. Persistence behind abstractions

Application defines persistence abstractions such as `IApplicationDbContext` and aggregate repositories. Infrastructure implements them with EF Core.

Repositories should expose domain-oriented operations and keep EF-specific types out of Domain.

## 5. Strongly typed identifiers

Aggregate identifiers use strongly typed ULID-backed value objects. String parsing belongs at application/API boundaries, not in core business logic.

## 6. Explicit aggregate relationships

Relationships are represented by IDs between aggregates rather than by cross-aggregate object graphs. This keeps aggregate boundaries explicit and avoids accidental transactional coupling.

## 7. Soft lifecycle before hard delete

For reference/master data such as Market and Venue, activation/deactivation is preferred over hard deletion when historical references may exist.

## 8. Consistent HTTP error mapping

Application returns `Result<T>` with stable error codes. API translates them centrally through `ApiErrorMapper`.

Error codes are part of the API contract and should remain stable once clients depend on them.

## 9. Server-side pagination

List endpoints use `PageRequest` and `PagedResult`. Normalization is centralized, and repositories perform `Count`, `OrderBy`, `Skip`, and `Take` in the database query.

## 10. Documentation reflects implementation state

A Domain model is not considered an implemented feature until the required Application, Infrastructure, persistence migration, API, and tests exist.
