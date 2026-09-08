// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Calendar.Enums
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Calendar.Enums;

/// <summary>
/// EN: Represents a configurable set of week days using bit flags.
/// FA: مجموعه قابل تنظیم روزهای هفته را به‌صورت Bit Flag نمایش می‌دهد.
/// </summary>
[Flags]
public enum TradingWeekDays
{
    None = 0,
    Sunday = 1 << 0,
    Monday = 1 << 1,
    Tuesday = 1 << 2,
    Wednesday = 1 << 3,
    Thursday = 1 << 4,
    Friday = 1 << 5,
    Saturday = 1 << 6
}
