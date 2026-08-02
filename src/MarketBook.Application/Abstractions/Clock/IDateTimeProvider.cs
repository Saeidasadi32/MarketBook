// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Clock
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Abstractions.Clock;

/// <summary>
/// EN: Provides access to current date and time.
/// FA: دسترسی به تاریخ و زمان جاری را فراهم می‌کند.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// EN: Gets current UTC date and time.
    /// FA: تاریخ و زمان فعلی UTC.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}