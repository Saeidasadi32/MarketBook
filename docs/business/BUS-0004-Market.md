# BUS-0004 — Market

## EN

A Market represents a logical trading market in MarketBook.

A Market may optionally be associated with an Exchange. A concrete execution/trading venue is represented separately by Venue.

Business rules currently implemented:

- Every Market has a unique `MarketId`.
- Every Market has a system-wide unique `MarketCode`.
- A Market has a display name.
- A Market may optionally reference an Exchange.
- A Market can be active or inactive.
- A Market can contain multiple Venues.
- Deactivation is preferred to hard deletion for normal lifecycle operations.

## FA

Market در MarketBook نمایانگر یک بازار معاملاتی منطقی است.

Market می‌تواند به‌صورت اختیاری به یک Exchange مرتبط باشد. بستر واقعی اجرای معامله به‌صورت مستقل با Venue مدل می‌شود.

قواعد فعلی:

- هر Market دارای `MarketId` یکتا است.
- هر Market دارای `MarketCode` یکتای سراسری است.
- Market دارای نام نمایشی است.
- ارتباط Market با Exchange اختیاری است.
- Market می‌تواند فعال یا غیرفعال باشد.
- هر Market می‌تواند چند Venue داشته باشد.
- در چرخه عمر عادی، غیرفعال‌سازی بر حذف فیزیکی ترجیح داده می‌شود.
