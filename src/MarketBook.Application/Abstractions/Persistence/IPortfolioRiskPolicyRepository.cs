// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Persistence
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.Aggregates;
using MarketBook.Domain.PortfolioRiskPolicy.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Defines persistence operations for portfolio risk-policy versions.
/// FA: عملیات ماندگاری نسخه‌های سیاست ریسک پرتفوی را تعریف می‌کند.
/// </summary>
public interface IPortfolioRiskPolicyRepository
{
    /// <summary>EN: Gets the active policy. FA: Policy فعال را دریافت می‌کند.</summary>
    Task<PortfolioRiskPolicy?> GetActiveAsync(PortfolioId portfolioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets the persisted non-draft policy effective at the supplied instant using half-open [from, to) semantics.
    /// FA: Policy ذخیره‌شده غیر Draft معتبر در لحظه داده‌شده را با منطق بازه نیمه‌باز [from, to) دریافت می‌کند.
    /// </summary>
    Task<PortfolioRiskPolicy?> GetEffectiveAsOfAsync(
        PortfolioId portfolioId,
        DateTimeOffset asOf,
        CancellationToken cancellationToken = default);

    /// <summary>EN: Gets a policy by identifier. FA: Policy را با شناسه دریافت می‌کند.</summary>
    Task<PortfolioRiskPolicy?> GetByIdAsync(PortfolioRiskPolicyId id, CancellationToken cancellationToken = default);

    /// <summary>EN: Gets a policy by business version. FA: Policy را با نسخه کسب‌وکاری دریافت می‌کند.</summary>
    Task<PortfolioRiskPolicy?> GetByVersionAsync(PortfolioId portfolioId, int policyVersion, CancellationToken cancellationToken = default);

    /// <summary>EN: Gets all versions ordered by version. FA: همه نسخه‌ها را مرتب‌شده دریافت می‌کند.</summary>
    Task<IReadOnlyCollection<PortfolioRiskPolicy>> GetHistoryAsync(PortfolioId portfolioId, CancellationToken cancellationToken = default);

    /// <summary>EN: Gets the next business version. FA: نسخه کسب‌وکاری بعدی را دریافت می‌کند.</summary>
    Task<int> GetNextVersionAsync(PortfolioId portfolioId, CancellationToken cancellationToken = default);

    /// <summary>EN: Adds a policy version. FA: یک نسخه Policy اضافه می‌کند.</summary>
    Task AddAsync(PortfolioRiskPolicy policy, CancellationToken cancellationToken = default);
}
