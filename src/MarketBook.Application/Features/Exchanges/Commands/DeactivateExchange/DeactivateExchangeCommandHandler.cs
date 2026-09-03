// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Commands.DeactivateExchange
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Exchange.Aggregates;
using MarketBook.Domain.Exchange.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Exchanges.Commands.DeactivateExchange;

/// <summary>
/// EN: Handles requests to deactivate an existing exchange.
/// FA: درخواست‌های غیرفعال‌سازی یک بورس موجود را پردازش می‌کند.
/// </summary>
public sealed class DeactivateExchangeCommandHandler
    : IRequestHandler<
        DeactivateExchangeCommand,
        Result<ExchangeId>>
{
    private readonly IExchangeRepository _exchangeRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new instance of the handler.
    /// FA: نمونه جدیدی از Handler را ایجاد می‌کند.
    /// </summary>
    /// <param name="exchangeRepository">
    /// EN: Exchange repository.
    /// FA: Repository مربوط به بورس.
    /// </param>
    /// <param name="dbContext">
    /// EN: Application database context.
    /// FA: کانتکست پایگاه داده برنامه.
    /// </param>
    public DeactivateExchangeCommandHandler(
        IExchangeRepository exchangeRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(exchangeRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _exchangeRepository = exchangeRepository;
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Result<ExchangeId>> Handle(
        DeactivateExchangeCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ExchangeId.TryParse(
                request.Id,
                out ExchangeId? exchangeId) ||
            exchangeId is null)
        {
            return Result<ExchangeId>.Fail(
                new Error(
                    "Exchange.InvalidId",
                    "The specified exchange identifier is invalid."));
        }

        Exchange? exchange =
            await _exchangeRepository.GetByIdAsync(
                exchangeId,
                cancellationToken);

        if (exchange is null)
        {
            return Result<ExchangeId>.Fail(
                new Error(
                    "Exchange.NotFound",
                    "The specified exchange was not found."));
        }

        exchange.Deactivate();

        _exchangeRepository.Update(exchange);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result<ExchangeId>.Success(exchange.Id);
    }
}
