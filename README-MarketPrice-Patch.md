# MarketBook MarketPrice / Price Limits Patch

Baseline: MarketBook(20260906-180202).zip (43/43 integration tests green).

Adds MarketPrice daily records tied to Listing, daily lower/upper price limits, create/update/get/paging API, EF mapping, migration and 6 integration tests.

Important: the migration class is explicit/discoverable and intentionally does not manually edit ApplicationDbContextModelSnapshot because this build environment has no .NET SDK. After the patch builds, verify the migration first with `dotnet ef migrations list` and an idempotent script before applying to the development database.
