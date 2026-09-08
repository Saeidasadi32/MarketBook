// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Commands.SetPortfolioBaseCurrency
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Commands.SetPortfolioBaseCurrency;

/// <summary>
/// EN: Handles portfolio base-currency changes.
/// FA: تغییر ارز پایه پرتفوی را مدیریت می‌کند.
/// </summary>
public sealed class SetPortfolioBaseCurrencyHandler
    : IRequestHandler<SetPortfolioBaseCurrencyCommand, Result<PortfolioId>>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public SetPortfolioBaseCurrencyHandler(
        IPortfolioRepository portfolioRepository,
        ICurrencyRepository currencyRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(portfolioRepository);
        ArgumentNullException.ThrowIfNull(currencyRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _portfolioRepository = portfolioRepository;
        _currencyRepository = currencyRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Sets an active currency as the portfolio base currency.
    /// FA: یک ارز فعال را به‌عنوان ارز پایه پرتفوی تنظیم می‌کند.
    /// </summary>
    public async Task<Result<PortfolioId>> Handle(
        SetPortfolioBaseCurrencyCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.InvalidId",
                    "The portfolio identifier is invalid."));
        }

        if (!CurrencyId.TryParse(request.CurrencyId, out CurrencyId? currencyId) ||
            currencyId is null)
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.InvalidBaseCurrencyId",
                    "The base-currency identifier is invalid."));
        }

        Portfolio? portfolio =
            await _portfolioRepository.GetByIdAsync(portfolioId, cancellationToken);

        if (portfolio is null)
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.NotFound",
                    "The portfolio was not found."));
        }

        Currency? currency =
            await _currencyRepository.GetByIdAsync(currencyId, cancellationToken);

        if (currency is null)
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.BaseCurrencyNotFound",
                    "The base currency was not found."));
        }

        if (!currency.IsActive)
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.BaseCurrencyInactive",
                    "The base currency must be active."));
        }

        portfolio.SetBaseCurrency(currencyId);
        _portfolioRepository.Update(portfolio);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<PortfolioId>.Success(portfolio.Id);
    }
}
