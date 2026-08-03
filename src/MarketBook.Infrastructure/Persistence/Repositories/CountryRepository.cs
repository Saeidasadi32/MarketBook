// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Repositories
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Infrastructure;
using MarketBook.Domain.Country.Aggregates;
using MarketBook.Domain.Country.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>
/// EN: Provides the Entity Framework Core implementation of
/// <see cref="ICountryRepository"/>.
///
/// FA: پیاده‌سازی Entity Framework Core برای
/// <see cref="ICountryRepository"/> را فراهم می‌کند.
/// </summary>
internal sealed class CountryRepository : ICountryRepository
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="CountryRepository"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="CountryRepository"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="context">
    /// EN: Application database context.
    /// FA: کانتکست پایگاه داده برنامه.
    /// </param>
    public CountryRepository(
        IApplicationDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Country?> GetByIdAsync(
        CountryId id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        return await _context.Set<Country>()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Country?> GetByCodeAsync(
        CountryCode code,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        return await _context.Set<Country>()
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(
        CountryCode code,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        return await _context.Set<Country>()
            .AnyAsync(
                x => x.Code == code,
                cancellationToken);
    }

    /// <inheritdoc/>
    public async Task AddAsync(
        Country country,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(country);

        await _context.Set<Country>()
            .AddAsync(
                country,
                cancellationToken);
    }

    /// <inheritdoc/>
    public void Remove(
        Country country)
    {
        ArgumentNullException.ThrowIfNull(country);

        _context.Set<Country>()
            .Remove(country);
    }
}
