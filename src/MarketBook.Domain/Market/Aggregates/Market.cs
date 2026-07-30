// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Market.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Exchange.ValueObjects;
using MarketBook.Domain.Market.ValueObjects;

namespace MarketBook.Domain.Market.Aggregates;

/// <summary>
/// EN: Represents a financial exchange or trading venue.
/// FA: یک بازار یا بورس مالی را نمایش می‌دهد.
/// </summary>
public sealed class Market : AggregateRoot<MarketId>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Market"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Market"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Market identifier.
    /// FA: شناسه بازار.
    /// </param>
    /// <param name="code">
    /// EN: Unique market code.
    /// FA: کد یکتای بازار.
    /// </param>
    /// <param name="name">
    /// EN: Market display name.
    /// FA: نام نمایشی بازار.
    /// </param>
    public Market(
        MarketId id,
        ExchangeId exchangeId,
        MarketCode code,
        string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name.Trim();
        ExchangeId = exchangeId;

        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Gets the market identifier.
    /// FA: شناسه بازار را دریافت می‌کند.
    /// </summary>
    public MarketId Id { get; }

    /// <summary>
    /// EN: Gets the market code.
    /// FA: کد بازار را دریافت می‌کند.
    /// </summary>
    public MarketCode Code { get; }
    
    /// <summary>
    /// EN: Gets exchange identifier.
    /// FA: شناسه بورس را دریافت می‌کند.
    /// </summary>
    public ExchangeId ExchangeId { get; }

    /// <summary>
    /// EN: Gets the market display name.
    /// FA: نام بازار را دریافت می‌کند.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// EN: Gets the creation date.
    /// FA: تاریخ ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets whether the market is active.
    /// FA: مشخص می‌کند بازار فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Renames the market.
    /// FA: نام بازار را تغییر می‌دهد.
    /// </summary>
    /// <param name="name">
    /// EN: New market name.
    /// FA: نام جدید بازار.
    /// </param>
    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
    }

    /// <summary>
    /// EN: Activates the market.
    /// FA: بازار را فعال می‌کند.
    /// </summary>
    public void Activate() => IsActive = true;

    /// <summary>
    /// EN: Deactivates the market.
    /// FA: بازار را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate() => IsActive = false;
}