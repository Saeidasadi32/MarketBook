// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Repositories
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Country.Aggregates;
using MarketBook.Domain.Country.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>
/// EN: Provides persistence operations for Country.
/// FA: عملیات ماندگاری Country را فراهم می‌کند.
/// </summary>
internal sealed class CountryRepository : ICountryRepository
{
    private readonly ApplicationDbContext _context;

    public CountryRepository(ApplicationDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public Task<Country?> GetByIdAsync(
        CountryId id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        return _context.Set<Country>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                country => country.Id == id,
                cancellationToken);
    }

    public Task<Country?> GetByCodeAsync(
        CountryCode code,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        return _context.Set<Country>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                country => country.Code == code,
                cancellationToken);
    }

    public Task<bool> ExistsAsync(
        CountryCode code,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        return _context.Set<Country>()
            .AnyAsync(
                country => country.Code == code,
                cancellationToken);
    }

    public async Task AddAsync(
        Country country,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(country);

        await _context.Set<Country>()
            .AddAsync(country, cancellationToken);
    }

    public void Remove(Country country)
    {
        ArgumentNullException.ThrowIfNull(country);

        _context.Set<Country>().Remove(country);
    }
}
