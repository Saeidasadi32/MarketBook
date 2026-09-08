// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioById;

/// <summary>
/// EN: Handles portfolio lookup by identifier.
/// FA: دریافت پرتفوی بر اساس شناسه را مدیریت می‌کند.
/// </summary>
public sealed class GetPortfolioByIdHandler
    : IRequestHandler<GetPortfolioByIdQuery, Result<GetPortfolioByIdResponse>>
{
    private readonly IPortfolioRepository _repository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetPortfolioByIdHandler(IPortfolioRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// EN: Handles the query.
    /// FA: Query را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetPortfolioByIdResponse>> Handle(
        GetPortfolioByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!PortfolioId.TryParse(request.Id, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioByIdResponse>.Fail(
                new Error(
                    "Portfolio.InvalidId",
                    "The portfolio identifier is invalid."));
        }

        Portfolio? portfolio =
            await _repository.GetByIdAsync(portfolioId, cancellationToken);

        if (portfolio is null)
        {
            return Result<GetPortfolioByIdResponse>.Fail(
                new Error(
                    "Portfolio.NotFound",
                    "The portfolio was not found."));
        }

        GetPortfolioByIdResponse response = new(
            portfolio.Id.Value.ToString(),
            portfolio.InvestorId.Value.ToString(),
            portfolio.Name.Value,
            portfolio.BaseCurrencyId?.Value.ToString(),
            portfolio.CreatedOn,
            portfolio.IsActive);

        return Result<GetPortfolioByIdResponse>.Success(response);
    }
}
