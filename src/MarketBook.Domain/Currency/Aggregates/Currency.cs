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
/// EN: Represents a currency used as a monetary unit for valuation,
/// settlement, or pricing.
/// FA: یک ارز را نمایش می‌دهد که به‌عنوان واحد پولی برای ارزش‌گذاری،
/// تسویه یا قیمت‌گذاری استفاده می‌شود.
/// </summary>
public sealed class Currency : AggregateRoot<CurrencyId>
{
    /// <summary>
    /// EN: Initializes a new instance of the <see cref="Currency"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="Currency"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Unique identifier of the currency.
    /// FA: شناسه یکتای ارز.
    /// </param>
    /// <param name="code">
    /// EN: Standard code identifying the currency.
    /// FA: کد استاندارد شناسایی ارز.
    /// </param>
    /// <param name="name">
    /// EN: Display name of the currency.
    /// FA: نام نمایشی ارز.
    /// </param>
    /// <param name="decimalPlaces">
    /// EN: Number of decimal places supported by the currency.
    /// FA: تعداد ارقام اعشاری قابل استفاده برای ارز.
    /// </param>
    public Currency(
        CurrencyId id,
        CurrencyCode code,
        string name,
        byte decimalPlaces)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (decimalPlaces > 18)
        {
            throw new ArgumentOutOfRangeException(
                nameof(decimalPlaces),
                "Decimal places must be between 0 and 18.");
        }

        Code = code;
        Name = name.Trim();
        DecimalPlaces = decimalPlaces;

        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Parameterless constructor for ORM frameworks.
    /// FA: سازنده بدون پارامتر برای فریم‌ورک‌های ORM.
    /// </summary>
    private Currency()
    {
        Code = default!;
        Name = default!;
    }

    /// <summary>
    /// EN: Gets the currency code.
    /// FA: کد ارز را دریافت می‌کند.
    /// </summary>
    public CurrencyCode Code { get; private set; }

    /// <summary>
    /// EN: Gets the display name of the currency.
    /// FA: نام نمایشی ارز را دریافت می‌کند.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// EN: Gets the number of decimal places supported by the currency.
    /// FA: تعداد ارقام اعشاری قابل استفاده برای ارز را دریافت می‌کند.
    /// </summary>
    public byte DecimalPlaces { get; private set; }

    /// <summary>
    /// EN: Gets the timestamp when the currency was created.
    /// FA: زمان ایجاد ارز را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>
    /// EN: Gets whether the currency is currently active.
    /// FA: مشخص می‌کند ارز در حال حاضر فعال است یا خیر.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Changes the display name of the currency.
    /// FA: نام نمایشی ارز را تغییر می‌دهد.
    /// </summary>
    /// <param name="name">
    /// EN: New currency name.
    /// FA: نام جدید ارز.
    /// </param>
    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
    }

    /// <summary>
    /// EN: Activates the currency.
    /// FA: ارز را فعال می‌کند.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
    }

    /// <summary>
    /// EN: Deactivates the currency.
    /// FA: ارز را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
    }
}
