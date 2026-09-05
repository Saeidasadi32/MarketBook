# BUS-0005 — Listing

## EN

A Listing represents an Instrument that is tradable at a specific Venue.

Finalized end-to-end rules:

- One Instrument can have many Listings.
- A Listing belongs to exactly one Instrument.
- A Listing belongs to exactly one Venue.
- A Listing uses exactly one quote Currency.
- `InstrumentId` and `VenueId` are immutable after creation.
- `QuoteCurrencyId`, `TradingSymbol`, `TickSize`, and `PricePrecision` can be updated.
- `TradingSymbol` is unique inside a Venue, enforced by the database on `(VenueId, TradingSymbol)`.
- The same symbol may therefore exist on different Venues.
- `TickSize` must be greater than zero.
- `PricePrecision` is between 0 and 28.
- A Listing can be active or inactive.
- One Instrument can have at most one primary Listing, enforced by a filtered unique database index.
- Primary status can be moved from one Listing to another for the same Instrument.

Create requires the referenced Instrument, Venue, and quote Currency to exist and be active.

Persistence uses restrictive foreign keys from Listing to Instrument, Venue, and Currency. These parent records cannot be deleted while referenced by a Listing.

## FA

Listing نمایانگر یک Instrument است که در یک Venue مشخص قابل معامله است.

قواعد نهایی Slice کامل Listing:

- هر Instrument می‌تواند چند Listing داشته باشد.
- هر Listing دقیقاً به یک Instrument تعلق دارد.
- هر Listing دقیقاً به یک Venue تعلق دارد.
- هر Listing دقیقاً یک Quote Currency دارد.
- `InstrumentId` و `VenueId` پس از ایجاد تغییرناپذیر هستند.
- `QuoteCurrencyId`، `TradingSymbol`، `TickSize` و `PricePrecision` قابل به‌روزرسانی هستند.
- `TradingSymbol` در محدوده هر Venue یکتا است و این قاعده با Unique Index روی `(VenueId, TradingSymbol)` در دیتابیس enforce می‌شود.
- بنابراین یک Symbol می‌تواند در Venueهای متفاوت تکرار شود.
- `TickSize` باید بزرگ‌تر از صفر باشد.
- `PricePrecision` بین 0 تا 28 است.
- Listing می‌تواند فعال یا غیرفعال باشد.
- هر Instrument حداکثر یک Listing اصلی دارد و این قاعده با filtered unique index در دیتابیس enforce می‌شود.
- وضعیت Primary می‌تواند از یک Listing به Listing دیگری از همان Instrument منتقل شود.

در زمان Create، Instrument، Venue و Quote Currency مرجع باید وجود داشته و فعال باشند.

کلیدهای خارجی Listing به Instrument، Venue و Currency از نوع Restrict هستند؛ بنابراین رکورد والد تا زمانی که توسط Listing استفاده می‌شود قابل حذف نیست.
