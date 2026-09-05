// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.Commands.DeactivateVenue
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Venue.Aggregates;
using MarketBook.Domain.Venue.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Venues.Commands.DeactivateVenue;

/// <summary>
/// EN: Handles deactivating an existing Venue aggregate.
/// FA: غیرفعال‌سازی Aggregate موجود محل معاملاتی را مدیریت می‌کند.
/// </summary>
public sealed class DeactivateVenueHandler
    : IRequestHandler<DeactivateVenueCommand, Result<VenueId>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new instance of the Deactivate Venue command handler.
    /// FA: یک نمونه جدید از Handler فرمان غیرفعال‌سازی محل معاملاتی را ایجاد می‌کند.
    /// </summary>
    /// <param name="venueRepository">
    /// EN: Venue repository.
    /// FA: Repository محل معاملاتی.
    /// </param>
    /// <param name="dbContext">
    /// EN: Application database context.
    /// FA: Context پایگاه داده برنامه.
    /// </param>
    public DeactivateVenueHandler(
        IVenueRepository venueRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(venueRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _venueRepository = venueRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the request to deactivate an existing venue.
    /// FA: درخواست غیرفعال‌سازی یک محل معاملاتی موجود را پردازش می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Deactivate Venue command.
    /// FA: فرمان غیرفعال‌سازی محل معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The identifier of the deactivated venue when successful; otherwise a domain error.
    /// FA: شناسه محل معاملاتی غیرفعال‌شده در صورت موفقیت؛ در غیر این صورت خطای دامنه.
    /// </returns>
    public async Task<Result<VenueId>> Handle(
        DeactivateVenueCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!VenueId.TryParse(
                request.Id,
                out VenueId? venueId) ||
            venueId is null)
        {
            return Result<VenueId>.Fail(
                new Error(
                    "Venue.InvalidId",
                    "The specified venue identifier is invalid."));
        }

        Venue? venue =
            await _venueRepository.GetByIdAsync(
                venueId,
                cancellationToken);

        if (venue is null)
        {
            return Result<VenueId>.Fail(
                new Error(
                    "Venue.NotFound",
                    "The specified venue was not found."));
        }

        venue.Deactivate();

        _venueRepository.Update(venue);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result<VenueId>.Success(venue.Id);
    }
}
