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
using MarketBook.Domain.Investor.Aggregates;
using MarketBook.Domain.Investor.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>EN: Defines persistence operations for Investor. FA: عملیات ماندگاری Investor را تعریف می‌کند.</summary>
public interface IInvestorRepository
{
    /// <summary>EN: Gets an investor by identifier. FA: سرمایه‌گذار را بر اساس شناسه دریافت می‌کند.</summary>
    Task<Investor?> GetByIdAsync(
        InvestorId id,
        CancellationToken cancellationToken = default);

    /// <summary>EN: Adds an investor. FA: سرمایه‌گذار را اضافه می‌کند.</summary>
    Task AddAsync(
        Investor investor,
        CancellationToken cancellationToken = default);

    /// <summary>EN: Marks an investor as modified. FA: سرمایه‌گذار را به‌عنوان تغییرکرده علامت‌گذاری می‌کند.</summary>
    void Update(Investor investor);

    /// <summary>EN: Gets investors with normalized pagination. FA: سرمایه‌گذاران را با صفحه‌بندی نرمال‌شده دریافت می‌کند.</summary>
    Task<PagedResult<Investor>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);
}
