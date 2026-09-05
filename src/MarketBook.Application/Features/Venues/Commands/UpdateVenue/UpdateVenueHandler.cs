// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.Commands.UpdateVenue
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Venue.Aggregates;
using MarketBook.Domain.Venue.Enums;
using MarketBook.Domain.Venue.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Venues.Commands.UpdateVenue;

/// <summary>
/// EN: Handles updating an existing Venue aggregate.
/// FA: به‌روزرسانی Aggregate موجود محل معاملاتی را مدیریت می‌کند.
/// </summary>
public sealed class UpdateVenueHandler
    : IRequestHandler<UpdateVenueCommand, Result<VenueId>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new instance of the Update Venue command handler.
    /// FA: یک نمونه جدید از Handler فرمان به‌روزرسانی محل معاملاتی را ایجاد می‌کند.
    /// </summary>
    /// <param name="venueRepository">
    /// EN: Venue repository.
    /// FA: Repository محل معاملاتی.
    /// </param>
    /// <param name="dbContext">
    /// EN: Application database context.
    /// FA: Context پایگاه داده برنامه.
    /// </param>
    public UpdateVenueHandler(
        IVenueRepository venueRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(venueRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _venueRepository = venueRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the request to update an existing venue.
    /// FA: درخواست به‌روزرسانی یک محل معاملاتی موجود را پردازش می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Update Venue command.
    /// FA: فرمان به‌روزرسانی محل معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The identifier of the updated venue when successful; otherwise a domain error.
    /// FA: شناسه محل معاملاتی به‌روزرسانی‌شده در صورت موفقیت؛ در غیر این صورت خطای دامنه.
    /// </returns>
    public async Task<Result<VenueId>> Handle(
        UpdateVenueCommand request,
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

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<VenueId>.Fail(
                new Error(
                    "Venue.InvalidName",
                    "Venue name is required."));
        }

        if (!Enum.IsDefined(
                typeof(VenueType),
                request.Type))
        {
            return Result<VenueId>.Fail(
                new Error(
                    "Venue.InvalidType",
                    "The specified venue type is invalid."));
        }

        venue.Rename(request.Name);
        venue.ChangeType((VenueType)request.Type);

        _venueRepository.Update(venue);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result<VenueId>.Success(venue.Id);
    }
}
