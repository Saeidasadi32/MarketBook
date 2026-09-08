// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Commands.DeactivatePortfolio
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;

using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Commands.DeactivatePortfolio;

/// <summary>
/// EN: Handles portfolio deactivate.
/// FA: غیرفعال‌سازی پرتفوی را مدیریت می‌کند.
/// </summary>
public sealed class DeactivatePortfolioHandler
    : IRequestHandler<DeactivatePortfolioCommand, Result<PortfolioId>>
{
    private readonly IPortfolioRepository _repository;

    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public DeactivatePortfolioHandler(
        IPortfolioRepository repository,

        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(repository);

        ArgumentNullException.ThrowIfNull(dbContext);
        _repository = repository;

        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the request.
    /// FA: درخواست را پردازش می‌کند.
    /// </summary>
    public async Task<Result<PortfolioId>> Handle(
        DeactivatePortfolioCommand request,
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

        portfolio.Deactivate();
        _repository.Update(portfolio);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<PortfolioId>.Success(portfolio.Id);
    }
}
