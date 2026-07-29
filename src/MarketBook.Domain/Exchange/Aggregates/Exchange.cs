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
/// EN: Represents a financial exchange.
/// FA: یک بورس یا صرافی مالی را نمایش می‌دهد.
/// </summary>
public sealed class Exchange : AggregateRoot
{
    /// <summary>
    /// EN: Initializes a new exchange.
    /// FA: یک بورس جدید ایجاد می‌کند.
    /// </summary>
    public Exchange(
        ExchangeId id,
        CountryId countryId,
        ExchangeCode code,
        string name)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Code = code;
        CountryId = countryId;
        Name = name.Trim();

        CreatedOn = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// EN: Gets exchange identifier.
    /// FA: شناسه بورس را دریافت می‌کند.
    /// </summary>
    public ExchangeId Id { get; }

    /// <summary>
    /// EN: Gets exchange code.
    /// FA: کد بورس را دریافت می‌کند.
    /// </summary>
    public ExchangeCode Code { get; }

    /// <summary>
    /// EN: Gets the country identifier.
    /// FA: شناسه کشور را دریافت می‌کند.
    /// </summary>
    public CountryId CountryId { get; }
    /// <summary>
    /// EN: Gets exchange name.
    /// FA: نام بورس را دریافت می‌کند.
    /// </summary>
    
    public string Name { get; private set; }

    /// <summary>
    /// EN: Gets creation time.
    /// FA: زمان ایجاد را دریافت می‌کند.
    /// </summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>
    /// EN: Gets active state.
    /// FA: وضعیت فعال بودن را دریافت می‌کند.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EN: Renames the exchange.
    /// FA: نام بورس را تغییر می‌دهد.
    /// </summary>
    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
    }

    /// <summary>
    /// EN: Activates the exchange.
    /// FA: بورس را فعال می‌کند.
    /// </summary>
    public void Activate() => IsActive = true;

    /// <summary>
    /// EN: Deactivates the exchange.
    /// FA: بورس را غیرفعال می‌کند.
    /// </summary>
    public void Deactivate() => IsActive = false;
}