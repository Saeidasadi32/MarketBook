# DOC-0026 — Time-Weighted Return Foundation / مبنای بازده زمانی‌وزن

Endpoint: `GET /api/v1/portfolios/{id}/twr?from=<DateTimeOffset>&to=<DateTimeOffset>`

- فقط Deposit و Withdrawal مرز TWR هستند.
- مرزهای هم‌زمان یک‌بار شمرده می‌شوند.
- NAV قبل از جریان در `OccurredOn - 1 tick` و NAV بعد از جریان در خود `OccurredOn` گرفته می‌شود.
- بازده هر زیر‌دوره: `EndingNAV / BeginningNAV - 1`.
- TWR نهایی: `Product(1+r_i)-1`.
- NAV ناقص، قیمت/FX گمشده یا Beginning NAV غیرمثبت باعث `null` شدن TWR می‌شود؛ صفر فرض نمی‌شود.
- Dividend/Interest/Fee/Tax/BuySettlement/SellSettlement/OtherCredit/OtherDebit مرز سرمایه خارجی نیستند.
- بدون Migration و بدون تغییر Safety Guard تست‌ها.

Deferred: annualization, benchmark, Modified Dietz, XIRR, event sequencing, stale-price policy, performance attribution.
