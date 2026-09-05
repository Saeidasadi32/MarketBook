# MarketBook Integration Tests Safety Fix 2

- Explicitly replaces `ApplicationDbContext` registration inside `WebApplicationFactory`.
- Keeps the database-name safety guard.
- Uses a unique database name prefixed with `MarketBookIntegrationTestsDb_`.
- Does not call `EnsureDeletedAsync` during initialization.
- Applies migrations only after the safety guard confirms the isolated test database.
- Deletes only the guarded isolated database during fixture cleanup.
- No `var` is used in the changed C# files.
