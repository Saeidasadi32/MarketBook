// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Currency.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.ValueObjects;

namespace MarketBook.Domain.Currency.Aggregates;

/// <summary>
/// EN: Represents a trading currency.
/// FA: یک ارز قابل استفاده در معاملات را نمایش می‌دهد.
/// </summary>
public sealed class Currency : AggregateRoot
{
    public Currency(
        CurrencyId id,
        CurrencyCode code,
        string name)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Code = code;
        Name = name.Trim();

        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Gets currency identifier.
    /// FA: شناسه ارز را دریافت می‌کند.
    /// </summary>
    public CurrencyId Id { get; }

    /// <summary>
    /// EN: Gets ISO currency code.
    /// FA: کد استاندارد ارز را دریافت می‌کند.
    /// </summary>
    public CurrencyCode Code { get; }

    /// <summary>
    /// EN: Gets currency name.
    /// FA: نام ارز را دریافت می‌کند.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// EN: Gets creation date.
    /// FA: تاریخ ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets active status.
    /// FA: وضعیت فعال بودن را دریافت می‌کند.
    /// </summary>
    public bool IsActive { get; private set; }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}