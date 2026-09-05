---
id: DOC-0005
title: Venue Domain Model
status: Implemented
version: 1.0.0
---

# Venue Domain Model

## Purpose

Venue represents a concrete trading/execution venue inside a logical Market.

## Identity and ownership

- `VenueId` — strongly typed ULID identifier.
- `MarketId` — required parent Market identifier.
- `VenueCode` — business code, normalized by its value object.

## State

- Name
- VenueType
- CreatedOn
- IsActive

## Current mutability policy

The current Aggregate allows:

- Rename
- ChangeType
- Activate
- Deactivate

The current Aggregate intentionally does **not** expose operations for changing:

- VenueCode
- MarketId

Changing either requires an explicit future Domain decision rather than bypassing the Aggregate with persistence setters.

## Domain events

- `VenueRenamedEvent`
- `VenueActivatedEvent`
- `VenueDeactivatedEvent`

## Application behavior

The end-to-end Venue slice currently supports:

- Create
- GetById
- GetAll with normalized pagination
- Update Name and Type
- Activate
- Deactivate

Creation validates that the referenced Market exists and is active.

## Persistence

Venue is stored with a required foreign key to Market. Delete behavior is restricted so a referenced Market is not cascaded through Venue relationships.
