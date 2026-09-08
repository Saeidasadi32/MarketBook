# MarketPrice Hotfix 1

Fixes the Domain compilation failure in MarketPrice.cs.

Root cause:
XML documentation comments and property declarations were accidentally written on the same physical line.
Because `///` comments consume the rest of the line, the C# compiler treated all those property declarations as comments.

Changes:
- Places every documented property declaration on its own line.
- No behavioral/domain-rule change.
- No migration change.
- No database update required.
- No `var` introduced.
