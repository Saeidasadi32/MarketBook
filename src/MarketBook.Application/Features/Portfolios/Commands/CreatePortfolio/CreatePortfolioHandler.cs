// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Commands.CreatePortfolio
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.Aggregates;
using MarketBook.Domain.Investor.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Commands.CreatePortfolio;

/// <summary>
/// EN: Handles portfolio creation.
/// FA: ایجاد پرتفوی را مدیریت می‌کند.
/// </summary>
public sealed class CreatePortfolioHandler
    : IRequestHandler<CreatePortfolioCommand, Result<PortfolioId>>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IInvestorRepository _investorRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public CreatePortfolioHandler(
        IPortfolioRepository portfolioRepository,
        IInvestorRepository investorRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(portfolioRepository);
        ArgumentNullException.ThrowIfNull(investorRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _portfolioRepository = portfolioRepository;
        _investorRepository = investorRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles portfolio creation.
    /// FA: ایجاد پرتفوی را پردازش می‌کند.
    /// </summary>
    public async Task<Result<PortfolioId>> Handle(
        CreatePortfolioCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!InvestorId.TryParse(request.InvestorId, out InvestorId? investorId) ||
            investorId is null)
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.InvalidInvestorId",
                    "The investor identifier is invalid."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.InvalidName",
                    "Portfolio name is required."));
        }

        Investor? investor =
            await _investorRepository.GetByIdAsync(investorId, cancellationToken);

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
                    "Cannot create a portfolio for an inactive investor."));
        }

        PortfolioName name = new(request.Name);

        bool duplicate =
            await _portfolioRepository.ExistsByInvestorAndNameAsync(
                investorId,
                name,
                null,
                cancellationToken);

        if (duplicate)
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.DuplicateName",
                    "The investor already has a portfolio with this name."));
        }

        Portfolio portfolio = Portfolio.Create(investorId, name);

        await _portfolioRepository.AddAsync(portfolio, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<PortfolioId>.Success(portfolio.Id);
    }
}
