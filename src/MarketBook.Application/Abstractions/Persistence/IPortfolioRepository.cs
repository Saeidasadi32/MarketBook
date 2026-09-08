// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Persistence
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Investor.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Defines persistence operations for portfolios.
/// FA: عملیات ماندگاری پرتفوی‌ها را تعریف می‌کند.
/// </summary>
public interface IPortfolioRepository
{
    /// <summary>
    /// EN: Gets a portfolio by identifier.
    /// FA: پرتفوی را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    Task<Portfolio?> GetByIdAsync(
        PortfolioId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Checks whether an investor already has a portfolio with the same name.
    /// FA: بررسی می‌کند آیا سرمایه‌گذار پرتفویی با همین نام دارد یا خیر.
    /// </summary>
    Task<bool> ExistsByInvestorAndNameAsync(
        InvestorId investorId,
        PortfolioName name,
        PortfolioId? excludingPortfolioId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Adds a portfolio.
    /// FA: یک پرتفوی اضافه می‌کند.
    /// </summary>
    Task AddAsync(
        Portfolio portfolio,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Marks a portfolio as modified.
    /// FA: پرتفوی را به‌عنوان تغییرکرده علامت‌گذاری می‌کند.
    /// </summary>
    void Update(Portfolio portfolio);

    /// <summary>
    /// EN: Gets portfolios with normalized pagination and optional investor filter.
    /// FA: پرتفوی‌ها را با صفحه‌بندی نرمال‌شده و فیلتر اختیاری سرمایه‌گذار دریافت می‌کند.
    /// </summary>
    Task<PagedResult<Portfolio>> GetPagedAsync(
        PageRequest pageRequest,
        InvestorId? investorId,
        CancellationToken cancellationToken = default);
}
