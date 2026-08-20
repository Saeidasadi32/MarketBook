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
/// EN: Represents a trading market that provides an environment for trading
/// financial instruments and their listings.
/// FA: یک بازار معاملاتی را نمایش می‌دهد که محیطی برای معامله ابزارهای مالی
/// و پذیرش‌های معاملاتی آن‌ها فراهم می‌کند.
/// </summary>
public sealed class Market : AggregateRoot<MarketId>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Market"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Market"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Unique identifier of the market.
    /// FA: شناسه یکتای بازار.
    /// </param>
    /// <param name="code">
    /// EN: Unique code identifying the market.
    /// FA: کد یکتای شناسایی بازار.
    /// </param>
    /// <param name="name">
    /// EN: Display name of the market.
    /// FA: نام نمایشی بازار.
    /// </param>
    /// <param name="exchangeId">
    /// EN: Optional exchange associated with the market.
    /// FA: شناسه بورس یا بستر معاملاتی مرتبط با بازار در صورت وجود.
    /// </param>
    public Market(
        MarketId id,
        MarketCode code,
        string name,
        ExchangeId? exchangeId = null)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Code = code;
        Name = name.Trim();
        ExchangeId = exchangeId;

        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private Market()
    {
        Code = default!;
        Name = default!;
    }

    /// <summary>
    /// EN: Gets the unique code of the market.
    /// FA: کد یکتای بازار را دریافت می‌کند.
    /// </summary>
    public MarketCode Code { get; private set; }

    /// <summary>
    /// EN: Gets the identifier of the associated exchange, when applicable.
    /// FA: شناسه بورس یا بستر معاملاتی مرتبط را در صورت وجود دریافت می‌کند.
    /// </summary>
    public ExchangeId? ExchangeId { get; private set; }

    /// <summary>
    /// EN: Gets the display name of the market.
    /// FA: نام نمایشی بازار را دریافت می‌کند.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// EN: Gets the creation timestamp of the market.
    /// FA: زمان ایجاد بازار را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>
    /// EN: Gets whether the market is currently active.
    /// FA: مشخص می‌کند بازار در حال حاضر فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Changes the display name of the market.
    /// FA: نام نمایشی بازار را تغییر می‌دهد.
    /// </summary>
    /// <param name="name">
    /// EN: The new market name.
    /// FA: نام جدید بازار.
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
    /// EN: Associates the market with an exchange.
    /// FA: بازار را به یک بورس یا بستر معاملاتی مرتبط می‌کند.
    /// </summary>
    /// <param name="exchangeId">
    /// EN: The exchange identifier.
    /// FA: شناسه بورس یا بستر معاملاتی.
    /// </param>
    public void AssignExchange(ExchangeId exchangeId)
    {
        ArgumentNullException.ThrowIfNull(exchangeId);

        ExchangeId = exchangeId;
    }

    /// <summary>
    /// EN: Removes the exchange association from the market.
    /// FA: ارتباط بازار با بورس یا بستر معاملاتی را حذف می‌کند.
    /// </summary>
    public void RemoveExchange()
    {
        ExchangeId = null;
    }

    /// <summary>
    /// EN: Activates the market.
    /// FA: بازار را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
    }

    /// <summary>
    /// EN: Deactivates the market.
    /// FA: بازار را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
    }

    /// <summary>
    /// EN: Creates a new market aggregate.
    /// FA: یک Aggregate جدید برای بازار ایجاد می‌کند.
    /// </summary>
    public static Market Create(
        MarketCode code,
        string name)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Market(
            MarketId.New(),
            code,
            name);
    }
}
