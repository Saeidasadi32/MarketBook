---
id: ADR-0003
title: Trading Hierarchy — Exchange, Market, Venue, Listing
status: Accepted
version: 1.0.0
---

# Context

MarketBook needs to represent organizational exchanges, logical markets, concrete trading venues, and tradable instrument listings without collapsing them into one concept.

Earlier business wording used "Market" and "Venue" interchangeably in places. The current Domain and persisted Venue implementation establish a clearer hierarchy.

# Decision

MarketBook uses the following model:

1. **Exchange** — an optional organizational/exchange-level parent. An Exchange may optionally belong to a Country.
2. **Market** — a logical market. A Market may optionally be associated with an Exchange.
3. **Venue** — a concrete execution/trading venue and must belong to a Market.
4. **Listing** — a tradable representation of an Instrument at a specific Venue and with a quote Currency.

Conceptually:

`Country -> Exchange -> Market -> Venue -> Listing`

The first two associations are optional where the Domain permits them; Venue-to-Market is required.

# Consequences

- Listing references `VenueId`, not `MarketId`.
- Market-level grouping can be derived through Venue.
- Symbol uniqueness rules must be finalized at the Listing/Venue scope before Listing persistence is implemented.
- `VenueCode` and `MarketId` are currently creation-time immutable.
- Business documentation must avoid describing Market itself as the concrete trading venue.

# Status implications

Country, Exchange, Market, and Venue are implemented end-to-end. Listing is Domain-modeled but not yet implemented end-to-end.
