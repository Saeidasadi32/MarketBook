# TradingCalendar Hotfix 3

Fixes all six TradingCalendar integration-test failures caused by FK cleanup order.

Root cause:
`ResetTradingCalendarsAsync()` deleted `Venue` rows while Listing tests had left
`Listings` that reference those Venues through `FK_Listings_Venues_VenueId`.

Fix:
The reset order is now:

1. Listings
2. TradingCalendars
3. Venues
4. Markets

The existing isolated-test-database safety guard remains unchanged and is still
executed before any delete operation.
