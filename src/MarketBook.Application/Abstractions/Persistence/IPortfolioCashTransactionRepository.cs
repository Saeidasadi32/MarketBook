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
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Persistence contract for immutable portfolio cash transactions.
/// FA: قرارداد ماندگاری تراکنش‌های تغییرناپذیر دفتر نقدی پرتفوی.
/// </summary>
public interface IPortfolioCashTransactionRepository
{
    /// <summary>EN: Gets a cash transaction by id. FA: تراکنش نقدی را بر اساس شناسه دریافت می‌کند.</summary>
    Task<PortfolioCashTransaction?> GetByIdAsync(
        PortfolioCashTransactionId id,
        CancellationToken cancellationToken = default);

    /// <summary>EN: Appends a cash transaction. FA: یک تراکنش نقدی را به دفتر اضافه می‌کند.</summary>
    Task AddAsync(
        PortfolioCashTransaction transaction,
        CancellationToken cancellationToken = default);

    /// <summary>EN: Gets paged cash transactions. FA: تراکنش‌های نقدی را به‌صورت صفحه‌بندی‌شده دریافت می‌کند.</summary>
    Task<PagedResult<PortfolioCashTransaction>> GetPagedAsync(
        PortfolioId portfolioId,
        CurrencyId? currencyId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);

    /// <summary>EN: Gets the deterministic ledger used for balance projection. FA: دفتر قطعی مورد استفاده برای محاسبه موجودی را دریافت می‌کند.</summary>
    Task<List<PortfolioCashTransaction>> GetLedgerAsync(
        PortfolioId portfolioId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets the deterministic cash ledger limited to an inclusive as-of instant.
    /// FA: دفتر قطعی تراکنش‌های نقدی را تا یک لحظه تاریخی شامل‌شونده دریافت می‌کند.
    /// </summary>
    /// <param name="portfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
    /// <param name="asOf">EN: Inclusive historical cutoff instant. FA: لحظه تاریخی شامل‌شونده برای برش Ledger.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>
    /// EN: Ordered cash ledger containing only transactions occurring on or before the cutoff.
    /// FA: دفتر مرتب تراکنش‌های نقدی شامل فقط مواردی که در لحظه برش یا قبل از آن رخ داده‌اند.
    /// </returns>
    Task<List<PortfolioCashTransaction>> GetLedgerAsync(
        PortfolioId portfolioId,
        DateTimeOffset asOf,
        CancellationToken cancellationToken = default);
}
