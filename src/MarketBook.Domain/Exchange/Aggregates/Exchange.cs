// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Exchange.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Country.ValueObjects;
using MarketBook.Domain.Exchange.ValueObjects;

namespace MarketBook.Domain.Exchange.Aggregates;

/// <summary>
/// EN: Represents an exchange or trading platform that provides one or more
/// trading markets.
/// FA: یک بورس یا بستر معاملاتی را نمایش می‌دهد که می‌تواند یک یا چند
/// بازار معاملاتی ارائه کند.
/// </summary>
public sealed class Exchange : AggregateRoot<ExchangeId>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Exchange"/> class.
    /// FA: یک نمونه جدید از کلاس <see cref="Exchange"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Unique exchange identifier.
    /// FA: شناسه یکتای بورس یا بستر معاملاتی.
    /// </param>
    /// <param name="code">
    /// EN: Unique exchange business code.
    /// FA: کد تجاری یکتای بورس یا بستر معاملاتی.
    /// </param>
    /// <param name="name">
    /// EN: Display name of the exchange.
    /// FA: نام نمایشی بورس یا بستر معاملاتی.
    /// </param>
    /// <param name="countryId">
    /// EN: Optional country associated with the exchange.
    /// FA: کشور مرتبط با بورس در صورت وجود؛ برای بسترهای جهانی اختیاری است.
    /// </param>
    public Exchange( 
        ExchangeId id,
        ExchangeCode code,
        string name,
        CountryId? countryId = null)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Code = code;
        Name = name.Trim();
        CountryId = countryId;

        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private Exchange()
    {
        Code = default!;
        Name = default!;
    }

    /// <summary>
    /// EN: Gets the unique exchange business code.
    /// FA: کد تجاری یکتای بورس یا بستر معاملاتی را دریافت می‌کند.
    /// </summary>
    public ExchangeCode Code { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the display name of the exchange.
    /// FA: نام نمایشی بورس یا بستر معاملاتی را دریافت می‌کند.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the associated country identifier, when applicable.
    /// FA: شناسه کشور مرتبط را در صورت وجود دریافت می‌کند.
    /// </summary>
    public CountryId? CountryId { get; private set; }

    /// <summary>
    /// EN: Gets the creation timestamp of the exchange.
    /// FA: زمان ایجاد بورس یا بستر معاملاتی را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets whether the exchange is currently active.
    /// FA: مشخص می‌کند بورس یا بستر معاملاتی در حال حاضر فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Renames the exchange.
    /// FA: نام بورس یا بستر معاملاتی را تغییر می‌دهد.
    /// </summary>
    /// <param name="name">
    /// EN: New exchange display name.
    /// FA: نام نمایشی جدید بورس.
    /// </param>
    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string normalizedName = name.Trim();

        if (Name == normalizedName)
            return;

        Name = normalizedName;

        Raise(new ExchangeRenamedEvent(
            Id,
            normalizedName));
    }

    /// <summary>
    /// EN: Associates the exchange with a country.
    /// FA: بورس یا بستر معاملاتی را به یک کشور مرتبط می‌کند.
    /// </summary>
    /// <param name="countryId">
    /// EN: Country identifier.
    /// FA: شناسه کشور.
    /// </param>
    public void AssignCountry(CountryId countryId)
    {
        ArgumentNullException.ThrowIfNull(countryId);

        if (CountryId == countryId)
            return;

        CountryId = countryId;

        Raise(new ExchangeCountryAssignedEvent(
            Id,
            countryId));
    }

    /// <summary>
    /// EN: Removes the country association from the exchange.
    /// FA: ارتباط کشور با بورس یا بستر معاملاتی را حذف می‌کند.
    /// </summary>
    public void RemoveCountry()
    {
        if (CountryId is null)
            return;

        CountryId = null;

        Raise(new ExchangeCountryRemovedEvent(Id));
    }

    /// <summary>
    /// EN: Activates the exchange.
    /// FA: بورس یا بستر معاملاتی را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;

        Raise(new ExchangeActivatedEvent(Id));
    }

    /// <summary>
    /// EN: Deactivates the exchange.
    /// FA: بورس یا بستر معاملاتی را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;

        Raise(new ExchangeDeactivatedEvent(Id));
    }

    /// <summary>
    /// EN: Creates a new exchange aggregate.
    /// FA: یک Aggregate جدید برای بورس یا بستر معاملاتی ایجاد می‌کند.
    /// </summary>
    public static Exchange Create(
        ExchangeCode code,
        string name,
        CountryId? countryId = null)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Exchange(
            ExchangeId.New(),
            code,
            name,
            countryId);
    }
    /// <summary>
    /// EN: Changes the business code of the exchange.
    /// FA: کد تجاری بورس یا بستر معاملاتی را تغییر می‌دهد.
    /// </summary>
    /// <param name="code">
    /// EN: New exchange business code.
    /// FA: کد تجاری جدید بورس.
    /// </param>
    public void ChangeCode(ExchangeCode code)
    {
        ArgumentNullException.ThrowIfNull(code);

        if (Code == code)
            return;

        Code = code;
    }
}

/// <summary>
/// EN: Raised when an exchange is renamed.
/// FA: زمانی که نام بورس تغییر می‌کند منتشر می‌شود.
/// </summary>
public sealed record ExchangeRenamedEvent(
    ExchangeId ExchangeId,
    string NewName) : DomainEvent;

/// <summary>
/// EN: Raised when a country is assigned to an exchange.
/// FA: زمانی که یک کشور به بورس اختصاص داده می‌شود منتشر می‌شود.
/// </summary>
public sealed record ExchangeCountryAssignedEvent(
    ExchangeId ExchangeId,
    CountryId CountryId) : DomainEvent;

/// <summary>
/// EN: Raised when the country association is removed.
/// FA: زمانی که ارتباط کشور با بورس حذف می‌شود منتشر می‌شود.
/// </summary>
public sealed record ExchangeCountryRemovedEvent(
    ExchangeId ExchangeId) : DomainEvent;

/// <summary>
/// EN: Raised when an exchange becomes active.
/// FA: زمانی که بورس فعال می‌شود منتشر می‌شود.
/// </summary>
public sealed record ExchangeActivatedEvent(
    ExchangeId ExchangeId) : DomainEvent;

/// <summary>
/// EN: Raised when an exchange becomes inactive.
/// FA: زمانی که بورس غیرفعال می‌شود منتشر می‌شود.
/// </summary>
public sealed record ExchangeDeactivatedEvent(
    ExchangeId ExchangeId) : DomainEvent;
