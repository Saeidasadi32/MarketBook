# MarketPrice / Price Limits Patch V2

Rebuilt replacement patch after compiler feedback.

Corrections in V2:
- XML documentation comments are placed on dedicated physical lines.
- C# declarations/methods are no longer swallowed by `///` comments.
- Corrected `MarketPrice.cs` property declarations are included.
- Existing integration-test database safety guard is preserved.
- No `var` is introduced in changed C# files.

Apply this ZIP over the project root after the original MarketPrice patch/hotfix.
No database update is required before the first build.
