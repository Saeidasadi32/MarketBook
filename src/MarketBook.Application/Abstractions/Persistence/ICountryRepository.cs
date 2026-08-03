// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Persistence
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Country.Aggregates;
using MarketBook.Domain.Country.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Defines persistence operations for the Countries aggregate.
/// FA: عملیات ماندگاری Aggregate کشور را تعریف می‌کند.
/// </summary>
public interface ICountryRepository
{
    /// <summary>
    /// EN: Gets a country by its identifier.
    /// FA: کشور را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    Task<Country?> GetByIdAsync(
        CountryId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets a country by ISO code.
    /// FA: کشور را بر اساس کد ISO دریافت می‌کند.
    /// </summary>
    Task<Country?> GetByCodeAsync(
        CountryCode code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Checks whether the specified ISO code already exists.
    /// FA: بررسی می‌کند آیا کد ISO از قبل وجود دارد یا خیر.
    /// </summary>
    Task<bool> ExistsAsync(
        CountryCode code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Adds a country.
    /// FA: یک کشور اضافه می‌کند.
    /// </summary>
    Task AddAsync(
        Country country,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Removes a country.
    /// FA: یک کشور حذف می‌کند.
    /// </summary>
    void Remove(Country country);
}
