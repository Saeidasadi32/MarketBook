// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Markets.Commands.UpdateMarket
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Exchange.Aggregates;
using MarketBook.Domain.Exchange.ValueObjects;
using MarketBook.Domain.Market.Aggregates;
using MarketBook.Domain.Market.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Markets.Commands.UpdateMarket;

/// <summary>
/// EN: Handles updating an existing Market aggregate.
/// FA: به‌روزرسانی Aggregate موجود بازار را مدیریت می‌کند.
/// </summary>
public sealed class UpdateMarketHandler
    : IRequestHandler<UpdateMarketCommand, Result<MarketId>>
{
    private readonly IMarketRepository _marketRepository;
    private readonly IExchangeRepository _exchangeRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new instance of the Update Market command handler.
    /// FA: یک نمونه جدید از Handler فرمان به‌روزرسانی بازار را ایجاد می‌کند.
    /// </summary>
    /// <param name="marketRepository">
    /// EN: Market repository.
    /// FA: Repository بازار.
    /// </param>
    /// <param name="exchangeRepository">
    /// EN: Exchange repository.
    /// FA: Repository بورس.
    /// </param>
    /// <param name="dbContext">
    /// EN: Application database context.
    /// FA: Context پایگاه داده برنامه.
    /// </param>
    public UpdateMarketHandler(
        IMarketRepository marketRepository,
        IExchangeRepository exchangeRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(marketRepository);
        ArgumentNullException.ThrowIfNull(exchangeRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _marketRepository = marketRepository;
        _exchangeRepository = exchangeRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the request to update an existing market.
    /// FA: درخواست به‌روزرسانی یک بازار موجود را پردازش می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Update Market command.
    /// FA: فرمان به‌روزرسانی بازار.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The identifier of the updated market when successful; otherwise a domain error.
    /// FA: شناسه بازار به‌روزرسانی‌شده در صورت موفقیت؛ در غیر این صورت خطای دامنه.
    /// </returns>
    public async Task<Result<MarketId>> Handle(
        UpdateMarketCommand request,
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

        MarketCode code;

        try
        {
            code = new MarketCode(request.Code);
        }
        catch (ArgumentException)
        {
            return Result<MarketId>.Fail(
                new Error(
                    "Market.InvalidCode",
                    "The specified market code is invalid."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<MarketId>.Fail(
                new Error(
                    "Market.InvalidName",
                    "Market name is required."));
        }

        if (await _marketRepository.ExistsAsync(
                code,
                marketId,
                cancellationToken))
        {
            return Result<MarketId>.Fail(
                new Error(
                    "Market.DuplicateCode",
                    "A market with the specified code already exists."));
        }

        market.ChangeCode(code);
        market.Rename(request.Name);

        if (string.IsNullOrWhiteSpace(request.ExchangeId))
        {
            market.RemoveExchange();
        }
        else
        {
            if (!ExchangeId.TryParse(
                    request.ExchangeId,
                    out ExchangeId? exchangeId) ||
                exchangeId is null)
            {
                return Result<MarketId>.Fail(
                    new Error(
                        "Market.InvalidExchangeId",
                        "The specified exchange identifier is invalid."));
            }

            Exchange? exchange =
                await _exchangeRepository.GetByIdAsync(
                    exchangeId,
                    cancellationToken);

            if (exchange is null)
            {
                return Result<MarketId>.Fail(
                    new Error(
                        "Market.ExchangeNotFound",
                        "The specified exchange was not found."));
            }

            if (!exchange.IsActive)
            {
                return Result<MarketId>.Fail(
                    new Error(
                        "Market.ExchangeInactive",
                        "The specified exchange is inactive and cannot be assigned to a market."));
            }

            market.AssignExchange(exchangeId);
        }

        _marketRepository.Update(market);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result<MarketId>.Success(market.Id);
    }
}
