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
    /// EN: Unique identifier of the instrument.
    /// FA: شناسه یکتای ابزار مالی.
    /// </param>
    /// <param name="name">
    /// EN: Instrument display name.
    /// FA: نام نمایشی ابزار مالی.
    /// </param>
    /// <param name="assetClass">
    /// EN: High-level asset class of the instrument.
    /// FA: کلاس اصلی دارایی ابزار مالی.
    /// </param>
    /// <param name="type">
    /// EN: Specific type of the instrument.
    /// FA: نوع مشخص ابزار مالی.
    /// </param>
    /// <param name="category">
    /// EN: High-level category of the instrument.
    /// FA: گروه اصلی ابزار مالی.
    /// </param>
    /// <param name="isin">
    /// EN: Optional International Securities Identification Number.
    /// FA: شناسه بین‌المللی اوراق بهادار که می‌تواند اختیاری باشد.
    /// </param>
    public Instrument(
        InstrumentId id,
        InstrumentName name,
        AssetClass assetClass,
        InstrumentType type,
        InstrumentCategory category,
        Isin? isin)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(assetClass);
        ArgumentNullException.ThrowIfNull(category);

        if (!Enum.IsDefined(typeof(InstrumentType), type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "The specified instrument type is invalid.");
        }

        Name = name;
        AssetClass = assetClass;
        Type = type;
        Category = category;
        Isin = isin;

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
    /// EN: Gets the high-level asset class.
    /// FA: کلاس اصلی دارایی را دریافت می‌کند.
    /// </summary>
    public AssetClass AssetClass { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the specific instrument type.
    /// FA: نوع مشخص ابزار مالی را دریافت می‌کند.
    /// </summary>
    public InstrumentType Type { get; private set; }

    /// <summary>
    /// EN: Gets the high-level instrument category.
    /// FA: گروه اصلی ابزار مالی را دریافت می‌کند.
    /// </summary>
    public InstrumentCategory Category { get; private set; } = default!;

    /// <summary>
    /// EN: Gets the optional International Securities Identification Number.
    /// FA: شناسه بین‌المللی اوراق بهادار اختیاری را دریافت می‌کند.
    /// </summary>
    public Isin? Isin { get; private set; }

    /// <summary>
    /// EN: Gets the instrument industry classification.
    /// FA: طبقه‌بندی صنعت ابزار مالی را دریافت می‌کند.
    /// </summary>
    public IndustryCategory? Industry { get; private set; }

    /// <summary>
    /// EN: Gets the economic sector of the instrument.
    /// FA: بخش اقتصادی ابزار مالی را دریافت می‌کند.
    /// </summary>
    public Sector.ValueObjects.Sector? Sector { get; private set; }

    /// <summary>
    /// EN: Gets the corporate aliases associated with the instrument.
    /// FA: نام‌های جایگزین شرکتی مرتبط با ابزار مالی را دریافت می‌کند.
    /// </summary>
    public CorporateAliases? CorporateAliases { get; private set; }

    /// <summary>
    /// EN: Gets the creation date and time of the instrument.
    /// FA: تاریخ و زمان ایجاد ابزار مالی را دریافت می‌کند.
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
    public void Rename(InstrumentName name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (Name == name)
            return;

        Name = name;

        Raise(new InstrumentRenamedEvent(Id, name));
    }

    /// <summary>
    /// EN: Changes the high-level asset class.
    /// FA: کلاس اصلی دارایی ابزار مالی را تغییر می‌دهد.
    /// </summary>
    /// <param name="assetClass">
    /// EN: New asset class.
    /// FA: کلاس دارایی جدید.
    /// </param>
    public void ChangeAssetClass(AssetClass assetClass)
    {
        ArgumentNullException.ThrowIfNull(assetClass);

        if (AssetClass == assetClass)
            return;

        AssetClass = assetClass;
    }

    /// <summary>
    /// EN: Changes the instrument type.
    /// FA: نوع ابزار مالی را تغییر می‌دهد.
    /// </summary>
    /// <param name="type">
    /// EN: New instrument type.
    /// FA: نوع جدید ابزار مالی.
    /// </param>
    public void ChangeType(InstrumentType type)
    {
        if (!Enum.IsDefined(typeof(InstrumentType), type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "The specified instrument type is invalid.");
        }

        if (Type == type)
            return;

        Type = type;
    }

    /// <summary>
    /// EN: Changes the high-level instrument category.
    /// FA: گروه اصلی ابزار مالی را تغییر می‌دهد.
    /// </summary>
    /// <param name="category">
    /// EN: New instrument category.
    /// FA: گروه جدید ابزار مالی.
    /// </param>
    public void ChangeCategory(InstrumentCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);

        if (Category == category)
            return;

        Category = category;
    }

    /// <summary>
    /// EN: Assigns or changes the instrument ISIN.
    /// FA: شناسه ISIN ابزار مالی را تعیین یا تغییر می‌دهد.
    /// </summary>
    /// <param name="isin">
    /// EN: New ISIN value.
    /// FA: مقدار جدید ISIN.
    /// </param>
    public void SetIsin(Isin isin)
    {
        ArgumentNullException.ThrowIfNull(isin);

        if (Isin == isin)
            return;

        Isin = isin;

        Raise(new InstrumentIsinChangedEvent(Id, isin));
    }

    /// <summary>
    /// EN: Removes the instrument ISIN when one is assigned.
    /// FA: در صورت وجود، ISIN ابزار مالی را حذف می‌کند.
    /// </summary>
    public void RemoveIsin()
    {
        if (Isin is null)
            return;

        Isin = null;
    }

    /// <summary>
    /// EN: Sets the industry classification of the instrument.
    /// FA: طبقه‌بندی صنعت ابزار مالی را تعیین می‌کند.
    /// </summary>
    /// <param name="industry">
    /// EN: Instrument industry.
    /// FA: صنعت ابزار مالی.
    /// </param>
    public void SetIndustry(IndustryCategory industry)
    {
        ArgumentNullException.ThrowIfNull(industry);

        if (Industry == industry)
            return;

        Industry = industry;

        Raise(new InstrumentIndustryChangedEvent(
            Id,
            industry,
            Sector));
    }

    /// <summary>
    /// EN: Sets the industry and economic sector of the instrument.
    /// FA: صنعت و بخش اقتصادی ابزار مالی را تعیین می‌کند.
    /// </summary>
    /// <param name="industry">
    /// EN: Instrument industry.
    /// FA: صنعت ابزار مالی.
    /// </param>
    /// <param name="sector">
    /// EN: Instrument economic sector.
    /// FA: بخش اقتصادی ابزار مالی.
    /// </param>
    public void SetIndustryAndSector(
        IndustryCategory industry,
        Sector.ValueObjects.Sector sector)
    {
        ArgumentNullException.ThrowIfNull(industry);
        ArgumentNullException.ThrowIfNull(sector);

        if (Industry == industry && Sector == sector)
            return;

        Industry = industry;
        Sector = sector;

        Raise(new InstrumentIndustryChangedEvent(
            Id,
            industry,
            sector));
    }

    /// <summary>
    /// EN: Adds a corporate alias to the instrument.
    /// FA: یک نام جایگزین شرکتی به ابزار مالی اضافه می‌کند.
    /// </summary>
    /// <param name="alias">
    /// EN: Corporate alias to add.
    /// FA: نام جایگزین شرکتی برای افزودن.
    /// </param>
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
    /// EN: Removes a corporate alias from the instrument.
    /// FA: یک نام جایگزین شرکتی را از ابزار مالی حذف می‌کند.
    /// </summary>
    /// <param name="alias">
    /// EN: Corporate alias to remove.
    /// FA: نام جایگزین شرکتی برای حذف.
    /// </param>
    public void RemoveCorporateAlias(string alias)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);

        if (CorporateAliases is null)
            return;

        if (!CorporateAliases.Remove(alias))
            return;

        Raise(new InstrumentAliasRemovedEvent(Id, alias));
    }

    /// <summary>
    /// EN: Replaces the corporate aliases of the instrument.
    /// FA: مجموعه نام‌های جایگزین شرکتی ابزار مالی را جایگزین می‌کند.
    /// </summary>
    /// <param name="aliases">
    /// EN: New corporate aliases.
    /// FA: نام‌های جایگزین شرکتی جدید.
    /// </param>
    public void SetCorporateAliases(IEnumerable<string> aliases)
    {
        ArgumentNullException.ThrowIfNull(aliases);

        CorporateAliases newAliases = new(aliases);

        if (CorporateAliases == newAliases)
            return;

        CorporateAliases = newAliases;

        Raise(new InstrumentAliasesSetEvent(
            Id,
            newAliases));
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
    /// EN: Creates a new instrument with a generated identifier.
    /// FA: یک ابزار مالی جدید با شناسه تولیدشده ایجاد می‌کند.
    /// </summary>
    /// <param name="name">
    /// EN: Instrument display name.
    /// FA: نام نمایشی ابزار مالی.
    /// </param>
    /// <param name="assetClass">
    /// EN: High-level asset class.
    /// FA: کلاس اصلی دارایی.
    /// </param>
    /// <param name="type">
    /// EN: Specific instrument type.
    /// FA: نوع مشخص ابزار مالی.
    /// </param>
    /// <param name="category">
    /// EN: High-level instrument category.
    /// FA: گروه اصلی ابزار مالی.
    /// </param>
    /// <param name="isin">
    /// EN: Optional ISIN.
    /// FA: ISIN اختیاری.
    /// </param>
    /// <returns>
    /// EN: A newly created instrument.
    /// FA: ابزار مالی جدید ایجادشده.
    /// </returns>
    public static Instrument Create(
        InstrumentName name,
        AssetClass assetClass,
        InstrumentType type,
        InstrumentCategory category,
        Isin? isin = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(assetClass);
        ArgumentNullException.ThrowIfNull(category);

        return new Instrument(
            InstrumentId.New(),
            name,
            assetClass,
            type,
            category,
            isin);
    }

    /// <summary>
    /// EN: Returns a string representation of the instrument.
    /// FA: نمایش رشته‌ای ابزار مالی را برمی‌گرداند.
    /// </summary>
    public override string ToString()
        => $"{Name} ({Type}) - {AssetClass}";
}

/// <summary>
/// EN: Domain event raised when an instrument is renamed.
/// FA: رویداد دامنه زمانی که نام ابزار مالی تغییر می‌کند.
/// </summary>
public sealed record InstrumentRenamedEvent(
    InstrumentId InstrumentId,
    InstrumentName NewName) : DomainEvent;

/// <summary>
/// EN: Domain event raised when an instrument ISIN is changed.
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

/// <summary>
/// EN: Domain event raised when an instrument industry classification changes.
/// FA: رویداد دامنه زمانی که طبقه‌بندی صنعت ابزار مالی تغییر می‌کند.
/// </summary>
public sealed record InstrumentIndustryChangedEvent(
    InstrumentId InstrumentId,
    IndustryCategory NewIndustry,
    Sector.ValueObjects.Sector? NewSector) : DomainEvent;

/// <summary>
/// EN: Domain event raised when a corporate alias is added to an instrument.
/// FA: رویداد دامنه زمانی که یک نام جایگزین شرکتی به ابزار مالی اضافه می‌شود.
/// </summary>
public sealed record InstrumentAliasAddedEvent(
    InstrumentId InstrumentId,
    string Alias) : DomainEvent;

/// <summary>
/// EN: Domain event raised when a corporate alias is removed from an instrument.
/// FA: رویداد دامنه زمانی که یک نام جایگزین شرکتی از ابزار مالی حذف می‌شود.
/// </summary>
public sealed record InstrumentAliasRemovedEvent(
    InstrumentId InstrumentId,
    string Alias) : DomainEvent;

/// <summary>
/// EN: Domain event raised when the corporate aliases of an instrument are replaced.
/// FA: رویداد دامنه زمانی که نام‌های جایگزین شرکتی ابزار مالی جایگزین می‌شوند.
/// </summary>
public sealed record InstrumentAliasesSetEvent(
    InstrumentId InstrumentId,
    CorporateAliases Aliases) : DomainEvent;
