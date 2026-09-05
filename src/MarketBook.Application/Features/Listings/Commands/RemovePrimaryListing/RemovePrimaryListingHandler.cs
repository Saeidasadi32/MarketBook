// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Commands.RemovePrimaryListing
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Listings.Commands.RemovePrimaryListing;

/// <summary>
/// EN: Handles the RemovePrimary Listing command.
/// FA: فرمان حذف وضعیت اصلی Listing را مدیریت می‌کند.
/// </summary>
public sealed class RemovePrimaryListingHandler
    : IRequestHandler<RemovePrimaryListingCommand, Result<ListingId>>
{
    private readonly IListingRepository _listingRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public RemovePrimaryListingHandler(
        IListingRepository listingRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(listingRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _listingRepository = listingRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the command.
    /// FA: فرمان را پردازش می‌کند.
    /// </summary>
    public async Task<Result<ListingId>> Handle(
        RemovePrimaryListingCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ListingId.TryParse(request.Id, out ListingId? listingId) ||
            listingId is null)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.InvalidId", "The specified listing identifier is invalid."));
        }

        Listing? listing =
            await _listingRepository.GetByIdAsync(listingId, cancellationToken);

        if (listing is null)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.NotFound", "The specified listing was not found."));
        }

        listing.RemovePrimary();
        _listingRepository.Update(listing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<ListingId>.Success(listing.Id);
    }
}
