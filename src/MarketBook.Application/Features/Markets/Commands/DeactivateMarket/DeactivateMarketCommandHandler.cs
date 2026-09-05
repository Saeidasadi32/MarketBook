// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Markets.Commands.DeactivateMarket
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Market.Aggregates;
using MarketBook.Domain.Market.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Markets.Commands.DeactivateMarket;

/// <summary>
/// EN: Handles deactivation of an existing Market aggregate.
/// FA: غیرفعال‌سازی Aggregate موجود بازار را مدیریت می‌کند.
/// </summary>
public sealed class DeactivateMarketCommandHandler
    : IRequestHandler<DeactivateMarketCommand, Result<MarketId>>
{
    private readonly IMarketRepository _marketRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new instance of the Deactivate Market command handler.
    /// FA: یک نمونه جدید از Handler فرمان غیرفعال‌سازی بازار را ایجاد می‌کند.
    /// </summary>
    /// <param name="marketRepository">
    /// EN: Market repository.
    /// FA: Repository بازار.
    /// </param>
    /// <param name="dbContext">
    /// EN: Application database context.
    /// FA: Context پایگاه داده برنامه.
    /// </param>
    public DeactivateMarketCommandHandler(
        IMarketRepository marketRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(marketRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _marketRepository = marketRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the request to deactivate an existing market.
    /// FA: درخواست غیرفعال‌سازی یک بازار موجود را پردازش می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Deactivate Market command.
    /// FA: فرمان غیرفعال‌سازی بازار.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The identifier of the deactivated market when successful; otherwise a domain error.
    /// FA: شناسه بازار غیرفعال‌شده در صورت موفقیت؛ در غیر این صورت خطای دامنه.
    /// </returns>
    public async Task<Result<MarketId>> Handle(
        DeactivateMarketCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!MarketId.TryParse(
                request.Id,
                out MarketId? marketId) ||
            marketId is null)
        {
            return Result<MarketId>.Fail(
                new Error(
                    "Market.InvalidId",
                    "The specified market identifier is invalid."));
        }

        Market? market =
            await _marketRepository.GetByIdAsync(
                marketId,
                cancellationToken);

        if (market is null)
        {
            return Result<MarketId>.Fail(
                new Error(
                    "Market.NotFound",
                    "The specified market was not found."));
        }

        market.Deactivate();

        _marketRepository.Update(market);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result<MarketId>.Success(market.Id);
    }
}
