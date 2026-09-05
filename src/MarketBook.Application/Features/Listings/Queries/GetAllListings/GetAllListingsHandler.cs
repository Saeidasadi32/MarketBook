// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Queries.GetAllListings
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.Aggregates;
using MediatR;

namespace MarketBook.Application.Features.Listings.Queries.GetAllListings;

/// <summary>
/// EN: Handles retrieving a paged collection of listings.
/// FA: دریافت مجموعه صفحه‌بندی‌شده Listingها را مدیریت می‌کند.
/// </summary>
public sealed class GetAllListingsHandler
    : IRequestHandler<GetAllListingsQuery, Result<GetAllListingsResponse>>
{
    private readonly IListingRepository _listingRepository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetAllListingsHandler(IListingRepository listingRepository)
    {
        ArgumentNullException.ThrowIfNull(listingRepository);
        _listingRepository = listingRepository;
    }

    /// <summary>
    /// EN: Handles the query.
    /// FA: پرس‌وجو را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetAllListingsResponse>> Handle(
        GetAllListingsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<Listing> pagedResult =
            await _listingRepository.GetPagedAsync(pageRequest, cancellationToken);

        ListingListItemResponse[] items =
            pagedResult.Items
                .Select(listing => new ListingListItemResponse(
                    listing.Id.Value.ToString(),
                    listing.InstrumentId.Value.ToString(),
                    listing.VenueId.Value.ToString(),
                    listing.QuoteCurrencyId.Value.ToString(),
                    listing.TradingSymbol.Value,
                    listing.TickSize,
                    listing.PricePrecision,
                    listing.IsPrimary,
                    listing.IsActive))
                .ToArray();

        GetAllListingsResponse response = new(
            items,
            pagedResult.Page,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages);

        return Result<GetAllListingsResponse>.Success(response);
    }
}
