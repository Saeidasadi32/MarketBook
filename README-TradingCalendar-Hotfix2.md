# TradingCalendar Hotfix 2

Fixes EF model validation error:

Navigation 'TradingCalendar (Dictionary<string, object>).DateExceptions' was not found.

Cause:
The manually edited model snapshot contained explicit owner-side
`b.Navigation("DateExceptions")` and `b.Navigation("Sessions")` declarations.
Those declarations do not match the owned-collection metadata represented by
the snapshot and made EF resolve the owner as a shared/dictionary entity.

Fix:
Removed those two invalid owner-side Navigation declarations.
The owned child-to-owner relationships remain in the snapshot.

No Domain/Application/API behavior changed.
The integration-test database Safety Guard was not changed.
