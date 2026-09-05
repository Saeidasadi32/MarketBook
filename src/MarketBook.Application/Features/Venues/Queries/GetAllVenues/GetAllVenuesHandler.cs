// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.Queries.GetAllVenues
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Common;
using MarketBook.Domain.Venue.Aggregates;
using MediatR;

namespace MarketBook.Application.Features.Venues.Queries.GetAllVenues;

/// <summary>
/// EN: Handles the query used to retrieve a paged collection of trading venues.
/// FA: پرس‌وجوی دریافت مجموعه‌ای صفحه‌بندی‌شده از بسترهای معاملاتی را پردازش می‌کند.
/// </summary>
public sealed class GetAllVenuesHandler
    : IRequestHandler<
        GetAllVenuesQuery,
        Result<GetAllVenuesResponse>>
{
    private readonly IVenueRepository _venueRepository;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="GetAllVenuesHandler"/> class.
    /// FA: یک نمونه جدید از کلاس <see cref="GetAllVenuesHandler"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="venueRepository">
    /// EN: Trading venue repository.
    /// FA: مخزن بسترهای معاملاتی.
    /// </param>
    public GetAllVenuesHandler(
        IVenueRepository venueRepository)
    {
        ArgumentNullException.ThrowIfNull(venueRepository);

        _venueRepository = venueRepository;
    }

    /// <summary>
    /// EN: Retrieves a paged collection of trading venues.
    /// FA: مجموعه‌ای صفحه‌بندی‌شده از بسترهای معاملاتی را دریافت می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Venue list query.
    /// FA: پرس‌وجوی فهرست بسترهای معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: A paged venue response.
    /// FA: پاسخ صفحه‌بندی‌شده بسترهای معاملاتی.
    /// </returns>
    public async Task<Result<GetAllVenuesResponse>> Handle(
        GetAllVenuesQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        PageRequest pageRequest =
            new()
            {
                Page = request.Page,
                PageSize = request.PageSize
            };

        PagedResult<Venue> pagedResult =
            await _venueRepository.GetPagedAsync(
                pageRequest,
                cancellationToken);

        IReadOnlyCollection<VenueListItemResponse> items =
            pagedResult.Items
                .Select(
                    venue =>
                        new VenueListItemResponse(
                            venue.Id.Value.ToString(),
                            venue.MarketId.Value.ToString(),
                            venue.Code.Value,
                            venue.Name,
                            (int)venue.Type,
                            venue.CreatedOn,
                            venue.IsActive))
                .ToArray();

        GetAllVenuesResponse response =
            new(
                items,
                pagedResult.Page,
                pagedResult.PageSize,
                pagedResult.TotalCount,
                pagedResult.TotalPages);

        return Result<GetAllVenuesResponse>.Success(
            response);
    }
}
