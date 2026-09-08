// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Queries.GetAllInvestors
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.Aggregates;
using MediatR;

namespace MarketBook.Application.Features.Investors.Queries.GetAllInvestors;

/// <summary>EN: Handles paged investor retrieval. FA: دریافت صفحه‌بندی‌شده سرمایه‌گذاران را مدیریت می‌کند.</summary>
public sealed class GetAllInvestorsHandler
    : IRequestHandler<GetAllInvestorsQuery, Result<GetAllInvestorsResponse>>
{
    private readonly IInvestorRepository _repository;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public GetAllInvestorsHandler(IInvestorRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>EN: Handles the query. FA: پرس‌وجو را پردازش می‌کند.</summary>
    public async Task<Result<GetAllInvestorsResponse>> Handle(
        GetAllInvestorsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<Investor> page =
            await _repository.GetPagedAsync(pageRequest, cancellationToken);

        InvestorListItemResponse[] items = page.Items
            .Select(item => new InvestorListItemResponse(
                item.Id.Value.ToString(),
                item.FullName,
                item.CreatedOn,
                item.IsActive))
            .ToArray();

        GetAllInvestorsResponse response = new(
            items,
            page.Page,
            page.PageSize,
            page.TotalCount,
            page.TotalPages);

        return Result<GetAllInvestorsResponse>.Success(response);
    }
}
