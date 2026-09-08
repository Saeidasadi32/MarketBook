// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.FxRates.Queries.GetLatestFxRate
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

namespace MarketBook.Application.Features.FxRates.Queries.GetLatestFxRate;

/// <summary>
/// EN: Handles latest exact-direction FX-rate queries.
/// FA: دریافت آخرین نرخ ارز با جهت دقیق را مدیریت می‌کند.
/// </summary>
public sealed class GetLatestFxRateHandler
    : IRequestHandler<GetLatestFxRateQuery, Result<GetLatestFxRateResponse>>
{
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetLatestFxRateHandler(IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Returns the most recent FX quote for the requested direction.
    /// FA: جدیدترین نرخ ارز را برای جهت درخواست‌شده برمی‌گرداند.
    /// </summary>
    public async Task<Result<GetLatestFxRateResponse>> Handle(
        GetLatestFxRateQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!CurrencyId.TryParse(request.BaseCurrencyId, out CurrencyId? baseCurrencyId) ||
            baseCurrencyId is null ||
            !CurrencyId.TryParse(request.QuoteCurrencyId, out CurrencyId? quoteCurrencyId) ||
            quoteCurrencyId is null)
        {
            return Result<GetLatestFxRateResponse>.Fail(
                new Error(
                    "FxRate.InvalidCurrencyId",
                    "Base or quote currency identifier is invalid."));
        }

        FxRate? fxRate =
            await _dbContext.Set<FxRate>()
                .AsNoTracking()
                .Where(
                    item =>
                        item.BaseCurrencyId == baseCurrencyId &&
                        item.QuoteCurrencyId == quoteCurrencyId)
                .OrderByDescending(item => item.RateDate)
                .ThenByDescending(item => item.CreatedOn)
                .FirstOrDefaultAsync(cancellationToken);

        if (fxRate is null)
        {
            return Result<GetLatestFxRateResponse>.Fail(
                new Error(
                    "FxRate.NotFound",
                    "No FX rate was found for the requested currency pair."));
        }

        return Result<GetLatestFxRateResponse>.Success(
            new GetLatestFxRateResponse(
                fxRate.Id.Value.ToString(),
                fxRate.BaseCurrencyId.Value.ToString(),
                fxRate.QuoteCurrencyId.Value.ToString(),
                fxRate.RateDate,
                fxRate.Rate,
                fxRate.CreatedOn));
    }
}
