# TradingCalendar Test Reset Hotfix 4

Built from the exact IntegrationTestFixture.cs uploaded from the user's machine.

Changes:
- ResetTradingCalendarsAsync now deletes Listings before TradingCalendars, Venues, and Markets.
- Removed the accidental duplicate Listing delete in ResetListingsAsync.
- Existing EnsureSafeTestDatabase guard is unchanged.
