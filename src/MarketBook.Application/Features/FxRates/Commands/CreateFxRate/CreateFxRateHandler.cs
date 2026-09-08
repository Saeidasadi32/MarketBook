// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.FxRates.Commands.CreateFxRate
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Application.Features.FxRates.Commands.CreateFxRate;

/// <summary>
/// EN: Handles FX-rate creation.
/// FA: ایجاد نرخ ارز را مدیریت می‌کند.
/// </summary>
public sealed class CreateFxRateHandler
    : IRequestHandler<CreateFxRateCommand, Result<FxRateId>>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public CreateFxRateHandler(
        ICurrencyRepository currencyRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(currencyRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _currencyRepository = currencyRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Creates a unique Base/Quote/date FX quote.
    /// FA: نرخ یکتای ارز برای ترکیب Base/Quote/تاریخ را ایجاد می‌کند.
    /// </summary>
    public async Task<Result<FxRateId>> Handle(
        CreateFxRateCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!CurrencyId.TryParse(request.BaseCurrencyId, out CurrencyId? baseCurrencyId) ||
            baseCurrencyId is null)
        {
            return Result<FxRateId>.Fail(
                new Error(
                    "FxRate.InvalidBaseCurrencyId",
                    "The base-currency identifier is invalid."));
        }

        if (!CurrencyId.TryParse(request.QuoteCurrencyId, out CurrencyId? quoteCurrencyId) ||
            quoteCurrencyId is null)
        {
            return Result<FxRateId>.Fail(
                new Error(
                    "FxRate.InvalidQuoteCurrencyId",
                    "The quote-currency identifier is invalid."));
        }

        if (baseCurrencyId == quoteCurrencyId)
        {
            return Result<FxRateId>.Fail(
                new Error(
                    "FxRate.SameCurrency",
                    "Base and quote currencies must be different."));
        }

        if (request.Rate <= 0m)
        {
            return Result<FxRateId>.Fail(
                new Error(
                    "FxRate.InvalidRate",
                    "FX rate must be greater than zero."));
        }

        Currency? baseCurrency =
            await _currencyRepository.GetByIdAsync(
                baseCurrencyId,
                cancellationToken);

        Currency? quoteCurrency =
            await _currencyRepository.GetByIdAsync(
                quoteCurrencyId,
                cancellationToken);

        if (baseCurrency is null || quoteCurrency is null)
        {
            return Result<FxRateId>.Fail(
                new Error(
                    "FxRate.CurrencyNotFound",
                    "Base or quote currency was not found."));
        }

        if (!baseCurrency.IsActive || !quoteCurrency.IsActive)
        {
            return Result<FxRateId>.Fail(
                new Error(
                    "FxRate.CurrencyInactive",
                    "Base and quote currencies must be active."));
        }

        bool duplicate =
            await _dbContext.Set<FxRate>()
                .AnyAsync(
                    item =>
                        item.BaseCurrencyId == baseCurrencyId &&
                        item.QuoteCurrencyId == quoteCurrencyId &&
                        item.RateDate == request.RateDate,
                    cancellationToken);

        if (duplicate)
        {
            return Result<FxRateId>.Fail(
                new Error(
                    "FxRate.DuplicatePairDate",
                    "An FX rate already exists for this currency pair and date."));
        }

        FxRate fxRate = FxRate.Create(
            baseCurrencyId,
            quoteCurrencyId,
            request.RateDate,
            request.Rate);

        await _dbContext.Set<FxRate>()
            .AddAsync(fxRate, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<FxRateId>.Success(fxRate.Id);
    }
}
