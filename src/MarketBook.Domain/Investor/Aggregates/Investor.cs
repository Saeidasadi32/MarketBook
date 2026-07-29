// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Investor.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.ValueObjects;

namespace MarketBook.Domain.Investor.Aggregates;

/// <summary>
/// EN: Represents an investor.
/// FA: یک سرمایه‌گذار را نمایش می‌دهد.
/// </summary>
public sealed class Investor : AggregateRoot
{
    /// <summary>
    /// EN: Initializes a new investor.
    /// FA: یک سرمایه‌گذار جدید ایجاد می‌کند.
    /// </summary>
    public Investor(
        InvestorId id,
        string fullName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

        Id = id;
        FullName = fullName.Trim();
        CreatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// EN: Gets the investor identifier.
    /// FA: شناسه سرمایه‌گذار را دریافت می‌کند.
    /// </summary>
    public InvestorId Id { get; }

    /// <summary>
    /// EN: Gets the investor full name.
    /// FA: نام کامل سرمایه‌گذار را دریافت می‌کند.
    /// </summary>
    public string FullName { get; private set; }

    /// <summary>
    /// EN: Gets the creation date.
    /// FA: تاریخ ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Changes the investor name.
    /// FA: نام سرمایه‌گذار را تغییر می‌دهد.
    /// </summary>
    public void Rename(string fullName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

        FullName = fullName.Trim();
    }
}