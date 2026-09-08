// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Commands.UpdatePortfolio
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Commands.UpdatePortfolio;

/// <summary>
/// EN: Handles portfolio updates.
/// FA: به‌روزرسانی پرتفوی را مدیریت می‌کند.
/// </summary>
public sealed class UpdatePortfolioHandler
    : IRequestHandler<UpdatePortfolioCommand, Result<PortfolioId>>
{
    private readonly IPortfolioRepository _repository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public UpdatePortfolioHandler(
        IPortfolioRepository repository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(dbContext);
        _repository = repository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles portfolio update.
    /// FA: به‌روزرسانی پرتفوی را پردازش می‌کند.
    /// </summary>
    public async Task<Result<PortfolioId>> Handle(
        UpdatePortfolioCommand request,
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

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.InvalidName",
                    "Portfolio name is required."));
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

        PortfolioName name = new(request.Name);

        bool duplicate =
            await _repository.ExistsByInvestorAndNameAsync(
                portfolio.InvestorId,
                name,
                portfolio.Id,
                cancellationToken);

        if (duplicate)
        {
            return Result<PortfolioId>.Fail(
                new Error(
                    "Portfolio.DuplicateName",
                    "The investor already has a portfolio with this name."));
        }

        portfolio.Rename(name);
        _repository.Update(portfolio);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<PortfolioId>.Success(portfolio.Id);
    }
}
