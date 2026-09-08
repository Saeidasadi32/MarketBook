# TradingCalendar Hotfix 1

Fixes the build error in `ApplicationDbContextModelSnapshot.cs`:

`EntityTypeBuilder does not contain a definition for Owned`

The two manually introduced `b.Owned();` calls were invalid for the EF Core 10 snapshot API and have been removed.

The later `Metadata file ... could not be found` messages are cascading build errors and should disappear once Infrastructure compiles.

No application/domain behavior was changed.
No integration-test database safety guard was changed.
