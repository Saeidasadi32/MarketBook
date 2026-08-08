// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Instrument.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Industry.ValueObjects;
using MarketBook.Domain.Instrument.Enums;
using MarketBook.Domain.Instrument.ValueObjects;

namespace MarketBook.Domain.Instrument.Aggregates;

/// <summary>
/// EN: Represents a tradable financial instrument.
/// FA: یک ابزار مالی قابل معامله را نمایش می‌دهد.
/// </summary>
public sealed class Instrument : AggregateRoot<InstrumentId>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Instrument"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Instrument"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Instrument identifier.
    /// FA: شناسه ابزار مالی.
    /// </param>
    /// <param name="name">
    /// EN: Instrument display name.
    /// FA: نام نمایشی ابزار مالی.
    /// </param>
    /// <param name="assetClass">
    /// EN: Asset class of the instrument.
    /// FA: کلاس دارایی ابزار مالی.
    /// </param>
    /// <param name="type">
    /// EN: Instrument type.
    /// FA: نوع ابزار مالی.
    /// </param>
    /// <param name="category">
    /// EN: Instrument category.
    /// FA: گروه ابزار مالی.
    /// </param>
    /// <exception cref="DomainException">
    /// EN: Thrown when any of the required parameters are invalid.
    /// FA: زمانی که هر یک از پارامترهای ضروری نامعتبر باشند پرتاب می‌شود.
    /// </exception>
    public Instrument(
        InstrumentId id,
        InstrumentName name,
        AssetClass assetClass,
        InstrumentType type,
        InstrumentCategory category)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(assetClass);
        ArgumentNullException.ThrowIfNull(category);

        Name = name;
        AssetClass = assetClass;
        Type = type;
        Category = category;

        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private Instrument()
    {
        // For ORM
    }

    /// <summary>
    /// EN: Gets the instrument display name.
    /// FA: نام نمایشی ابزار مالی را دریافت می‌کند.
    /// </summary>
    public InstrumentName Name { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the asset class.
    /// FA: کلاس دارایی را دریافت می‌کند.
    /// </summary>
    public AssetClass AssetClass { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the instrument type.
    /// FA: نوع ابزار مالی را دریافت می‌کند.
    /// </summary>
    public InstrumentType Type { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the instrument category.
    /// FA: گروه ابزار مالی را دریافت می‌کند.
    /// </summary>
    public InstrumentCategory Category { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the International Securities Identification Number (ISIN).
    /// FA: شناسه بین‌المللی اوراق بهادار (ISIN) را دریافت می‌کند.
    /// </summary>
    public Isin? Isin { get; private set; }

    /// <summary>
    /// EN: Gets the creation date and time.
    /// FA: تاریخ و زمان ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets a value indicating whether the instrument is active.
    /// FA: مشخص می‌کند که ابزار مالی فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Changes the instrument display name.
    /// FA: نام نمایشی ابزار مالی را تغییر می‌دهد.
    /// </summary>
    /// <param name="name">
    /// EN: New instrument name.
    /// FA: نام جدید ابزار مالی.
    /// </param>
    /// <exception cref="DomainException">
    /// EN: Thrown when the name is null or empty.
    /// FA: زمانی که نام null یا خالی باشد پرتاب می‌شود.
    /// </exception>
    public void Rename(InstrumentName name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (string.IsNullOrWhiteSpace(name.Value))
        {
            throw new DomainException(
                new Error(
                    "Instrument.Name.Empty",
                    "Instrument name cannot be empty."));
        }

        if (Name == name)
            return;

        Name = name;
        Raise(new InstrumentRenamedEvent(Id, name));
    }

    /// <summary>
    /// EN: Assigns or changes the ISIN.
    /// FA: شناسه ISIN را تعیین یا تغییر می‌دهد.
    /// </summary>
    /// <param name="isin">
    /// EN: ISIN value.
    /// FA: مقدار ISIN.
    /// </param>
    /// <exception cref="DomainException">
    /// EN: Thrown when the ISIN is null or invalid.
    /// FA: زمانی که ISIN null یا نامعتبر باشد پرتاب می‌شود.
    /// </exception>
    public void SetIsin(Isin isin)
    {
        ArgumentNullException.ThrowIfNull(isin);

        if (string.IsNullOrWhiteSpace(isin.Value))
        {
            throw new DomainException(
                new Error(
                    "Instrument.Isin.Invalid",
                    "ISIN cannot be empty."));
        }

        if (Isin == isin)
            return;

        Isin = isin;
        Raise(new InstrumentIsinChangedEvent(Id, isin));
    }

    /// <summary>
    /// EN: Activates the instrument.
    /// FA: ابزار مالی را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
        Raise(new InstrumentActivatedEvent(Id));
    }

    /// <summary>
    /// EN: Deactivates the instrument.
    /// FA: ابزار مالی را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        Raise(new InstrumentDeactivatedEvent(Id));
    }

    /// <summary>
    /// EN: Returns a string representation of the instrument.
    /// FA: نمایش رشته‌ای از ابزار مالی را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => $"{Name} ({Type}) - {AssetClass}";

    /// <summary>
    /// EN: Gets the industry.
    /// FA: صنعت را دریافت می‌کند.
    /// </summary>
    public Industry.ValueObjects.IndustryCategory? Industry { get; private set; }

    /// <summary>
    /// EN: Gets the sector.
    /// FA: بخش اقتصادی را دریافت می‌کند.
    /// </summary>
    public Sector.ValueObjects.Sector? Sector { get; private set; }

    /// <summary>
    /// EN: Gets the corporate aliases.
    /// FA: نام‌های جایگزین شرکت را دریافت می‌کند.
    /// </summary>
    public CorporateAliases? CorporateAliases { get; private set; }

    /// <summary>
    /// EN: Sets the industry.
    /// FA: صنعت را تنظیم می‌کند.
    /// </summary>
    public void SetIndustry(Industry.ValueObjects.IndustryCategory industry)
    {
        ArgumentNullException.ThrowIfNull(industry);

        if (Industry == industry)
            return;

        Industry = industry;
        Raise(new InstrumentIndustrySetEvent(Id, industry));
    }

    /// <summary>
    /// EN: Sets the industry and sector.
    /// FA: صنعت و بخش اقتصادی را تنظیم می‌کند.
    /// </summary>
    public void SetIndustryAndSector(Industry.ValueObjects.IndustryCategory industry, Sector.ValueObjects.Sector sector)
    {
        ArgumentNullException.ThrowIfNull(industry);
        ArgumentNullException.ThrowIfNull(sector);

        if (Industry == industry && Sector == sector)
            return;

        Industry = industry;
        Sector = sector;
        Raise(new InstrumentIndustryChangedEvent(Id, industry, sector));
    }

    /// <summary>
    /// EN: Adds a corporate alias.
    /// FA: یک نام جایگزین برای شرکت اضافه می‌کند.
    /// </summary>
    public void AddCorporateAlias(string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);

        CorporateAliases ??= new CorporateAliases();

        if (CorporateAliases.Contains(alias))
            return;

        CorporateAliases.Add(alias);
        Raise(new InstrumentAliasAddedEvent(Id, alias));
    }

    /// <summary>
    /// EN: Removes a corporate alias.
    /// FA: یک نام جایگزین برای شرکت حذف می‌کند.
    /// </summary>
    public void RemoveCorporateAlias(string alias)
    {
        if (CorporateAliases is null)
            return;

        if (CorporateAliases.Remove(alias))
        {
            Raise(new InstrumentAliasRemovedEvent(Id, alias));
        }
    }

    /// <summary>
    /// EN: Sets multiple corporate aliases.
    /// FA: چندین نام جایگزین برای شرکت تنظیم می‌کند.
    /// </summary>
    public void SetCorporateAliases(IEnumerable<string> aliases)
    {
        ArgumentNullException.ThrowIfNull(aliases);

        var newAliases = new CorporateAliases(aliases);

        if (CorporateAliases == newAliases)
            return;

        CorporateAliases = newAliases;
        Raise(new InstrumentAliasesSetEvent(Id, newAliases));
    }
}

/// <summary>
/// EN: Domain event raised when an instrument is renamed.
/// FA: رویداد دامنه زمانی که نام ابزار مالی تغییر می‌کند.
/// </summary>
public sealed record InstrumentRenamedEvent(
    InstrumentId InstrumentId,
    InstrumentName NewName) : DomainEvent;

/// <summary>
/// EN: Domain event raised when an instrument's ISIN is changed.
/// FA: رویداد دامنه زمانی که ISIN ابزار مالی تغییر می‌کند.
/// </summary>
public sealed record InstrumentIsinChangedEvent(
    InstrumentId InstrumentId,
    Isin NewIsin) : DomainEvent;

/// <summary>
/// EN: Domain event raised when an instrument is activated.
/// FA: رویداد دامنه زمانی که ابزار مالی فعال می‌شود.
/// </summary>
public sealed record InstrumentActivatedEvent(
    InstrumentId InstrumentId) : DomainEvent;

/// <summary>
/// EN: Domain event raised when an instrument is deactivated.
/// FA: رویداد دامنه زمانی که ابزار مالی غیرفعال می‌شود.
/// </summary>
public sealed record InstrumentDeactivatedEvent(
    InstrumentId InstrumentId) : DomainEvent;

// Domain Events جدید
public sealed record InstrumentIndustryChangedEvent(
    InstrumentId InstrumentId,
    Industry.ValueObjects.IndustryCategory NewIndustry,
    Sector.ValueObjects.Sector NewSector) : DomainEvent;

public sealed record InstrumentAliasAddedEvent(
    InstrumentId InstrumentId,
    string Alias) : DomainEvent;

public sealed record InstrumentAliasRemovedEvent(
    InstrumentId InstrumentId,
    string Alias) : DomainEvent;

public sealed record InstrumentAliasesSetEvent(
    InstrumentId InstrumentId,
    CorporateAliases Aliases) : DomainEvent;

// Domain Events جدید
public sealed record InstrumentIndustrySetEvent(
    InstrumentId InstrumentId,
    Industry.ValueObjects.IndustryCategory Industry) : DomainEvent;

public sealed record InstrumentAliasesUpdatedEvent(
    InstrumentId InstrumentId,
    CorporateAliases Aliases) : DomainEvent;
