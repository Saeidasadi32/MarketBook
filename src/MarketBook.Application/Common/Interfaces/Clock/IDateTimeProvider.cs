// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Common.Interfaces.Clock
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Common.Interfaces.Clock;

/// <summary>
/// EN: Provides the current date and time.
///
/// FA: تاریخ و زمان جاری را فراهم می‌کند.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// EN: Gets current UTC date and time.
    ///
    /// FA: تاریخ و زمان فعلی UTC را دریافت می‌کند.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}