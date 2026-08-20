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
/// FA: یک کشور مستقل را در سیستم نمایش می‌دهد.
/// </summary>
public sealed class Country : AggregateRoot<CountryId>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Country"/> class.
    /// FA: یک نمونه جدید از کلاس <see cref="Country"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Unique country identifier.
    /// FA: شناسه یکتای کشور.
    /// </param>
    /// <param name="code">
    /// EN: ISO country code.
    /// FA: کد استاندارد ISO کشور.
    /// </param>
    /// <param name="name">
    /// EN: Country display name.
    /// FA: نام نمایشی کشور.
    /// </param>
    /// <param name="timeZone">
    /// EN: Default time zone associated with the country.
    /// FA: منطقه زمانی پیش‌فرض مرتبط با کشور.
    /// </param>
    public Country(
        CountryId id,
        CountryCode code,
        string name,
        TimeZoneId timeZone)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(timeZone);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Code = code;
        Name = name.Trim();
        TimeZone = timeZone;

        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private Country()
    {
        Code = default!;
        Name = default!;
        TimeZone = default!;
    }

    /// <summary>
    /// EN: Gets the ISO country code.
    /// FA: کد استاندارد ISO کشور را دریافت می‌کند.
    /// </summary>
    public CountryCode Code { get; }

    /// <summary>
    /// EN: Gets the country display name.
    /// FA: نام نمایشی کشور را دریافت می‌کند.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// EN: Gets the default time zone of the country.
    /// FA: منطقه زمانی پیش‌فرض کشور را دریافت می‌کند.
    /// </summary>
    public TimeZoneId TimeZone { get; }

    /// <summary>
    /// EN: Gets the date and time when the country was created.
    /// FA: تاریخ و زمان ایجاد کشور را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets a value indicating whether the country is active.
    /// FA: مشخص می‌کند که کشور فعال است یا خیر.
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

        string normalizedName = name.Trim();

        if (Name == normalizedName)
            return;

        Name = normalizedName;
    }

    /// <summary>
    /// EN: Activates the country.
    /// FA: کشور را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
    }

    /// <summary>
    /// EN: Deactivates the country.
    /// FA: کشور را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
    }

    /// <summary>
    /// EN: Creates a new country aggregate.
    /// FA: یک Aggregate جدید برای کشور ایجاد می‌کند.
    /// </summary>
    public static Country Create(
        CountryCode code,
        string name,
        TimeZoneId timeZone)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(timeZone);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Country(
            CountryId.New(),
            code,
            name,
            timeZone);
    }
}
