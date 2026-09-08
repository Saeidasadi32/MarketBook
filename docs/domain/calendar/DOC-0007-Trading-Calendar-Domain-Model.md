# DOC-0007 — Trading Calendar and Trading Session

Status date: 2026-09-06

## Purpose

`TradingCalendar` defines whether a date is tradable for one Market in one Gregorian year and stores that market's regular weekly trading sessions.

## Key design decision: weekends are configurable

The previous Domain prototype hard-coded Saturday and Sunday as weekends.

That assumption has been removed.

`WeekendDays` is now a bit-flag configuration stored on each yearly TradingCalendar. Therefore different markets can define different non-trading week days without code changes.

Examples:

- Friday only
- Friday + Saturday
- Saturday + Sunday
- any other valid market-specific combination

At least one day of the week must remain potentially tradable.

## Identity and uniqueness

A TradingCalendar has a strongly typed ULID `TradingCalendarId`.

Database uniqueness:

`(MarketId, Year)`

A Market can therefore have only one TradingCalendar for a given year.

## Trading sessions

A `TradingSession` defines regular market-local opening and closing times for one `DayOfWeek`.

Current slice deliberately supports one regular session per week day.

Rules:

- close time must be after open time
- a regular session cannot be configured on a configured weekend day
- one session per day per calendar

Split sessions / lunch breaks can be added later if a concrete market requires them.

## Date exceptions

A `CalendarDateException` represents either:

- full holiday
- half-day

A date cannot be both.

Date exceptions must belong to the TradingCalendar year.

A half-day cannot be configured on a weekend date.

## Day evaluation

`IsTradingDay(date)` evaluates:

1. the date belongs to the calendar year
2. it is not a configured full holiday
3. its DayOfWeek is not in configured WeekendDays

`IsHalfDay(date)` is evaluated independently from the date-exception collection.

`GetSession(date)` returns the regular weekly session only when the date is a trading day.

## Persistence

Tables:

- `TradingCalendars`
- `TradingCalendarSessions`
- `TradingCalendarDateExceptions`

Foreign keys:

- TradingCalendar -> Market: Restrict
- child Session/DateException -> TradingCalendar: Cascade

## API

Base route:

`/api/v1/trading-calendars`

Operations:

- POST Create
- GET by id
- GET paged
- PUT replace mutable configuration
- PATCH activate/deactivate
- GET `{id}/days/{date}` to evaluate trading status and session

## Integration tests

The SQL Server integration suite verifies configurable weekends explicitly: a calendar configured with Friday as its only weekend treats Saturday as a trading day and Friday as non-trading.

This is the regression test that prevents reintroduction of the old Saturday/Sunday hard-code.
