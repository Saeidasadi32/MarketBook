// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Country.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Common.ValueObjects;
using MarketBook.Domain.Country.ValueObjects;

namespace MarketBook.Domain.Country.Aggregates;

/// <summary>
/// EN: Represents a sovereign country.
/// FA: یک کشور را در سیستم نمایش می‌دهد.
/// </summary>
public sealed class Country : AggregateRoot<CountryId>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Country"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Country"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Country identifier.
    /// FA: شناسه کشور.
    /// </param>
    /// <param name="code">
    /// EN: ISO country code.
    /// FA: کد استاندارد کشور.
    /// </param>
    /// <param name="name">
    /// EN: Country display name.
    /// FA: نام کشور.
    /// </param>
    public Country(
        CountryId id,
        CountryCode code,
        string name,
        TimeZoneId timeZone)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Code = code;
        Name = name.Trim();
        TimeZone = timeZone;

        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Gets the ISO country code.
    /// FA: کد استاندارد کشور را دریافت می‌کند.
    /// </summary>
    public CountryCode Code { get; }

    /// <summary>
    /// EN: Gets the country name.
    /// FA: نام کشور را دریافت می‌کند.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// EN: Gets the default time zone.
    /// FA: منطقه زمانی پیش‌فرض کشور را دریافت می‌کند.
    /// </summary>
    public TimeZoneId TimeZone { get; }

    /// <summary>
    /// EN: Gets the creation date.
    /// FA: تاریخ ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets a value indicating whether the country is active.
    /// FA: مشخص می‌کند کشور فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Renames the country.
    /// FA: نام کشور را تغییر می‌دهد.
    /// </summary>
    /// <param name="name">
    /// EN: New country name.
    /// FA: نام جدید کشور.
    /// </param>
    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
    }

    /// <summary>
    /// EN: Activates the country.
    /// FA: کشور را فعال می‌کند.
    /// </summary>
    public void Activate() => IsActive = true;

    /// <summary>
    /// EN: Deactivates the country.
    /// FA: کشور را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate() => IsActive = false;
}
