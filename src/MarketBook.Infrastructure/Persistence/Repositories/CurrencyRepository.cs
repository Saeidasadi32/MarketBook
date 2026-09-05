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
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>
/// EN: Implements persistence operations for the Currency aggregate.
/// FA: عملیات ذخیره‌سازی Aggregate مربوط به ارز معاملاتی را پیاده‌سازی می‌کند.
/// </summary>
public sealed class CurrencyRepository : ICurrencyRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="CurrencyRepository"/> class.
    /// FA: یک نمونه جدید از کلاس <see cref="CurrencyRepository"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="dbContext">
    /// EN: Application database context.
    /// FA: Context پایگاه داده برنامه.
    /// </param>
    public CurrencyRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Gets a currency by its identifier.
    /// FA: یک ارز معاملاتی را بر اساس شناسه آن دریافت می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Currency identifier.
    /// FA: شناسه ارز معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The currency if found; otherwise null.
    /// FA: در صورت یافتن، ارز معاملاتی و در غیر این صورت null.
    /// </returns>
    public async Task<Currency?> GetByIdAsync(
        CurrencyId id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        return await _dbContext.Set<Currency>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                currency => currency.Id == id,
                cancellationToken);
    }

    /// <summary>
    /// EN: Gets a currency by its business code.
    /// FA: یک ارز معاملاتی را بر اساس کد تجاری آن دریافت می‌کند.
    /// </summary>
    /// <param name="code">
    /// EN: Currency business code.
    /// FA: کد تجاری ارز معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The currency if found; otherwise null.
    /// FA: در صورت یافتن، ارز معاملاتی و در غیر این صورت null.
    /// </returns>
    public async Task<Currency?> GetByCodeAsync(
        CurrencyCode code,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        return await _dbContext.Set<Currency>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                currency => currency.Code == code,
                cancellationToken);
    }

    /// <summary>
    /// EN: Determines whether a currency with the specified code exists.
    /// FA: بررسی می‌کند آیا ارزی با کد مشخص‌شده وجود دارد یا خیر.
    /// </summary>
    /// <param name="code">
    /// EN: Currency business code.
    /// FA: کد تجاری ارز معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: True if a matching currency exists; otherwise false.
    /// FA: اگر ارز معاملاتی مطابق وجود داشته باشد true و در غیر این صورت false.
    /// </returns>
    public async Task<bool> ExistsAsync(
        CurrencyCode code,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        return await _dbContext.Set<Currency>()
            .AnyAsync(
                currency => currency.Code == code,
                cancellationToken);
    }

    /// <summary>
    /// EN: Determines whether another currency with the specified code exists,
    /// excluding the specified currency.
    /// FA: بررسی می‌کند آیا ارز معاملاتی دیگری با کد مشخص‌شده،
    /// به‌جز ارز تعیین‌شده، وجود دارد یا خیر.
    /// </summary>
    /// <param name="code">
    /// EN: Currency business code.
    /// FA: کد تجاری ارز معاملاتی.
    /// </param>
    /// <param name="excludingId">
    /// EN: Currency identifier to exclude from the search.
    /// FA: شناسه ارزی که باید از جستجو مستثنی شود.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: True if another matching currency exists; otherwise false.
    /// FA: اگر ارز دیگری با کد مشخص‌شده وجود داشته باشد true و در غیر این صورت false.
    /// </returns>
    public async Task<bool> ExistsAsync(
        CurrencyCode code,
        CurrencyId excludingId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(excludingId);

        return await _dbContext.Set<Currency>()
            .AnyAsync(
                currency =>
                    currency.Code == code &&
                    currency.Id != excludingId,
                cancellationToken);
    }

    /// <summary>
    /// EN: Adds a new currency to the persistence context.
    /// FA: یک ارز معاملاتی جدید را به Context ذخیره‌سازی اضافه می‌کند.
    /// </summary>
    /// <param name="currency">
    /// EN: Currency aggregate to add.
    /// FA: Aggregate ارز معاملاتی برای افزودن.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    public async Task AddAsync(
        Currency currency,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(currency);

        await _dbContext.Set<Currency>()
            .AddAsync(currency, cancellationToken);
    }

    /// <summary>
    /// EN: Marks a currency as modified in the persistence context.
    /// FA: یک ارز معاملاتی را به‌عنوان تغییرکرده در Context ذخیره‌سازی علامت‌گذاری می‌کند.
    /// </summary>
    /// <param name="currency">
    /// EN: Currency aggregate to update.
    /// FA: Aggregate ارز معاملاتی برای به‌روزرسانی.
    /// </param>
    public void Update(Currency currency)
    {
        ArgumentNullException.ThrowIfNull(currency);

        _dbContext.Set<Currency>().Update(currency);
    }

    /// <summary>
    /// EN: Marks a currency for removal from persistence.
    /// FA: یک ارز معاملاتی را برای حذف از ذخیره‌سازی علامت‌گذاری می‌کند.
    /// </summary>
    /// <param name="currency">
    /// EN: Currency aggregate to remove.
    /// FA: Aggregate ارز معاملاتی برای حذف.
    /// </param>
    public void Remove(Currency currency)
    {
        ArgumentNullException.ThrowIfNull(currency);

        _dbContext.Set<Currency>().Remove(currency);
    }

    /// <summary>
    /// EN: Gets a paged collection of currencys.
    /// FA: مجموعه‌ای صفحه‌بندی‌شده از ارزها را دریافت می‌کند.
    /// </summary>
    /// <param name="pageRequest">
    /// EN: Pagination parameters.
    /// FA: پارامترهای صفحه‌بندی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: A paged result containing the requested currencys.
    /// FA: نتیجه صفحه‌بندی‌شده شامل ارزها مورد درخواست.
    /// </returns>
    public async Task<PagedResult<Currency>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pageRequest);

        int normalizedPage = pageRequest.NormalizedPage;
        int normalizedPageSize = pageRequest.NormalizedPageSize;

        IQueryable<Currency> query = _dbContext.Set<Currency>()
            .AsNoTracking()
            .OrderBy(currency => currency.Code);

        int totalCount =
            await query.CountAsync(cancellationToken);

        List<Currency> items =
            await query
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToListAsync(cancellationToken);

        return new PagedResult<Currency>(
            items,
            normalizedPage,
            normalizedPageSize,
            totalCount);
    }
}
