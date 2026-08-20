// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Venue.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Market.ValueObjects;
using MarketBook.Domain.Venue.ValueObjects;

namespace MarketBook.Domain.Venue.Aggregates;

/// <summary>
/// EN: Represents a trading venue where financial instruments can be traded
/// within a logical market.
/// FA: یک بستر معاملاتی را نمایش می‌دهد که ابزارهای مالی در یک بازار منطقی
/// در آن قابل معامله هستند.
/// </summary>
public sealed class Venue : AggregateRoot<VenueId>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Venue"/> class.
    /// FA: یک نمونه جدید از کلاس <see cref="Venue"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Unique venue identifier.
    /// FA: شناسه یکتای بستر معاملاتی.
    /// </param>
    /// <param name="marketId">
    /// EN: Identifier of the logical market to which the venue belongs.
    /// FA: شناسه بازار منطقی که بستر معاملاتی به آن تعلق دارد.
    /// </param>
    /// <param name="code">
    /// EN: Unique business code of the venue.
    /// FA: کد تجاری یکتای بستر معاملاتی.
    /// </param>
    /// <param name="name">
    /// EN: Display name of the venue.
    /// FA: نام نمایشی بستر معاملاتی.
    /// </param>
    public Venue(
        VenueId id,
        MarketId marketId,
        VenueCode code,
        string name)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(marketId);
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        MarketId = marketId;
        Code = code;
        Name = name.Trim();

        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private Venue()
    {
        MarketId = default!;
        Code = default!;
        Name = default!;
    }

    /// <summary>
    /// EN: Gets the identifier of the logical market.
    /// FA: شناسه بازار منطقی را دریافت می‌کند.
    /// </summary>
    public MarketId MarketId { get; }

    /// <summary>
    /// EN: Gets the business code of the venue.
    /// FA: کد تجاری بستر معاملاتی را دریافت می‌کند.
    /// </summary>
    public VenueCode Code { get; }

    /// <summary>
    /// EN: Gets the display name of the venue.
    /// FA: نام نمایشی بستر معاملاتی را دریافت می‌کند.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// EN: Gets the creation timestamp of the venue.
    /// FA: زمان ایجاد بستر معاملاتی را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets a value indicating whether the venue is active.
    /// FA: مشخص می‌کند بستر معاملاتی فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Renames the venue.
    /// FA: نام بستر معاملاتی را تغییر می‌دهد.
    /// </summary>
    /// <param name="name">
    /// EN: New venue name.
    /// FA: نام جدید بستر معاملاتی.
    /// </param>
    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalizedName = name.Trim();

        if (Name == normalizedName)
            return;

        Name = normalizedName;
    }

    /// <summary>
    /// EN: Activates the venue.
    /// FA: بستر معاملاتی را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
    }

    /// <summary>
    /// EN: Deactivates the venue.
    /// FA: بستر معاملاتی را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
    }

    /// <summary>
    /// EN: Creates a new venue aggregate.
    /// FA: یک Aggregate جدید برای بستر معاملاتی ایجاد می‌کند.
    /// </summary>
    public static Venue Create(
        MarketId marketId,
        VenueCode code,
        string name)
    {
        ArgumentNullException.ThrowIfNull(marketId);
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Venue(
            VenueId.New(),
            marketId,
            code,
            name);
    }
}
