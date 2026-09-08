// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetAllPortfolios
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetAllPortfolios;

/// <summary>
/// EN: Handles paged portfolio queries.
/// FA: Query صفحه‌بندی‌شده پرتفوی‌ها را مدیریت می‌کند.
/// </summary>
public sealed class GetAllPortfoliosHandler
    : IRequestHandler<GetAllPortfoliosQuery, Result<GetAllPortfoliosResponse>>
{
    private readonly IPortfolioRepository _repository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetAllPortfoliosHandler(IPortfolioRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// EN: Handles the query.
    /// FA: Query را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetAllPortfoliosResponse>> Handle(
        GetAllPortfoliosQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        InvestorId? investorId = null;

        if (!string.IsNullOrWhiteSpace(request.InvestorId))
        {
            if (!InvestorId.TryParse(request.InvestorId, out investorId) ||
                investorId is null)
            {
                return Result<GetAllPortfoliosResponse>.Fail(
                    new Error(
                        "Portfolio.InvalidInvestorId",
                        "The investor identifier is invalid."));
            }
        }

        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<Portfolio> result =
            await _repository.GetPagedAsync(
                pageRequest,
                investorId,
                cancellationToken);

        List<PortfolioListItemResponse> items = result.Items
            .Select(item => new PortfolioListItemResponse(
                item.Id.Value.ToString(),
                item.InvestorId.Value.ToString(),
                item.Name.Value,
                item.CreatedOn,
                item.IsActive))
            .ToList();

        GetAllPortfoliosResponse response = new(
            items,
            result.Page,
            result.PageSize,
            result.TotalCount);

        return Result<GetAllPortfoliosResponse>.Success(response);
    }
}
