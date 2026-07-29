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
using MarketBook.Domain.Instrument.Enums;
using MarketBook.Domain.Instrument.ValueObjects;

namespace MarketBook.Domain.Instrument.Aggregates;

/// <summary>
/// EN: Represents a tradable financial instrument.
/// FA: یک ابزار مالی قابل معامله را نمایش می‌دهد.
/// </summary>
public sealed class Instrument : AggregateRoot
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
    /// <param name="type">
    /// EN: Instrument type.
    /// FA: نوع ابزار مالی.
    /// </param>
    public Instrument(
        InstrumentId id,
        InstrumentName name,
        AssetClass assetClass,
        InstrumentType type,
        InstrumentCategory category)
    {
        Id = id;
        Name = name;
        Type = type;
        Category = category;
        AssetClass = assetClass;

        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Gets the unique instrument identifier.
    /// FA: شناسه یکتای ابزار مالی را دریافت می‌کند.
    /// </summary>
    public InstrumentId Id { get; }

    /// <summary>
    /// EN: Gets the instrument display name.
    /// FA: نام نمایشی ابزار مالی را دریافت می‌کند.
    /// </summary>
    public InstrumentName Name { get; private set; }

    /// <summary>
    /// EN: Gets the instrument type.
    /// FA: نوع ابزار مالی را دریافت می‌کند.
    /// </summary>
    public InstrumentType Type { get; }

    /// <summary>
    /// EN: Gets the asset class.
    /// FA: کلاس دارایی را دریافت می‌کند.
    /// </summary>
    public AssetClass AssetClass { get; }

    /// <summary>
    /// EN: Gets the instrument category.
    /// FA: گروه ابزار مالی را دریافت می‌کند.
    /// </summary>
    public InstrumentCategory Category { get; }

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
    public void Rename(InstrumentName name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
    }

    /// <summary>
    /// EN: Assigns or changes the ISIN.
    /// FA: شناسه ISIN را تعیین یا تغییر می‌دهد.
    /// </summary>
    /// <param name="isin">
    /// EN: ISIN value.
    /// FA: مقدار ISIN.
    /// </param>
    public void SetIsin(Isin isin)
    {
        ArgumentNullException.ThrowIfNull(isin);

        Isin = isin;
    }

    /// <summary>
    /// EN: Activates the instrument.
    /// FA: ابزار مالی را فعال می‌کند.
    /// </summary>
    public void Activate() => IsActive = true;

    /// <summary>
    /// EN: Deactivates the instrument.
    /// FA: ابزار مالی را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate() => IsActive = false;
}