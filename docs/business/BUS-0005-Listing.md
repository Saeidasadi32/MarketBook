# BUS-0005 — Listing

## EN

A Listing represents an Instrument that is tradable at a specific Venue.

Current Domain model:

- One Instrument can have many Listings.
- A Listing belongs to one Venue.
- A Listing uses one quote Currency.
- A Listing owns its TradingSymbol, TickSize, and PricePrecision.
- A Listing can be active or inactive.
- A Listing can be marked as primary.

The exact database uniqueness scope for TradingSymbol must be finalized before the Listing end-to-end slice is implemented. The current Domain model alone does not establish a persisted uniqueness constraint.

## FA

Listing نمایانگر یک Instrument است که در یک Venue مشخص قابل معامله است.

مدل فعلی Domain:

- هر Instrument می‌تواند چند Listing داشته باشد.
- هر Listing به یک Venue تعلق دارد.
- هر Listing یک ارز مظنه (Quote Currency) دارد.
- TradingSymbol، TickSize و PricePrecision از ویژگی‌های خود Listing هستند.
- Listing می‌تواند فعال یا غیرفعال باشد.
- Listing می‌تواند به‌عنوان پذیرش اصلی علامت‌گذاری شود.

دامنه دقیق یکتایی TradingSymbol باید پیش از پیاده‌سازی end-to-end Listing نهایی شود؛ مدل Domain فعلی به‌تنهایی محدودیت یکتایی پایگاه داده را تعیین نمی‌کند.
