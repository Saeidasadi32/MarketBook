# MarketBook Integration Tests Patch

This patch adds automated Currency API integration tests to the existing
`MarketBook.Integration.Tests` project.

## Behavior

- Uses `WebApplicationFactory<Program>` to host the real API pipeline.
- Uses SQL Server, matching production persistence behavior.
- Reuses the configured SQL Server connection string but changes only the
  database name to `MarketBookIntegrationTestsDb`.
- Applies EF Core migrations automatically before the integration-test
  collection starts.
- Deletes the integration-test database when the collection finishes.
- Never uses the normal `MarketBookDb` database for test data.
- Currency scenarios reset only the `Currencies` table between tests.
- No `var` is used in added or changed C# files.

## Run

```powershell
dotnet build
dotnet test
```

To run only the integration test project:

```powershell
dotnet test tests/MarketBook.Integration.Tests/MarketBook.Integration.Tests.csproj
```
