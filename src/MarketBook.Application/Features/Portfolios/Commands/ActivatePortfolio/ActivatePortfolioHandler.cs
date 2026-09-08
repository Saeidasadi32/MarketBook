// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Commands.ActivatePortfolio
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.Aggregates;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Commands.ActivatePortfolio;

/// <summary>
/// EN: Handles portfolio activate.
/// FA: فعال‌سازی پرتفوی را مدیریت می‌کند.
/// </summary>
public sealed class ActivatePortfolioHandler
    : IRequestHandler<ActivatePortfolioCommand, Result<PortfolioId>>
{
    private readonly IPortfolioRepository _repository;
    private readonly IInvestorRepository _investorRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public ActivatePortfolioHandler(
        IPortfolioRepository repository,
        IInvestorRepository investorRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(investorRepository);
        ArgumentNullException.ThrowIfNull(dbContext);
        _repository = repository;
        _investorRepository = investorRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the request.
    /// FA: درخواست را پردازش می‌کند.
    /// </summary>
    public async Task<Result<PortfolioId>> Handle(
        ActivatePortfolioCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!PortfolioId.TryParse(request.Id, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.InvalidId",
                    "The portfolio identifier is invalid."));
        }

        Portfolio? portfolio =
            await _repository.GetByIdAsync(portfolioId, cancellationToken);

        if (portfolio is null)
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.NotFound",
                    "The portfolio was not found."));
        }

        Investor? investor =
            await _investorRepository.GetByIdAsync(
                portfolio.InvestorId,
                cancellationToken);

        if (investor is null)
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.InvestorNotFound",
                    "The investor was not found."));
        }

        if (!investor.IsActive)
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.InvestorInactive",
                    "Cannot activate a portfolio for an inactive investor."));
        }

        portfolio.Activate();
        _repository.Update(portfolio);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<PortfolioId>.Success(portfolio.Id);
    }
}
