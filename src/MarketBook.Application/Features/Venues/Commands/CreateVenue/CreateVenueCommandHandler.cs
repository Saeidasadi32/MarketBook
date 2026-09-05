// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.Commands.CreateVenue
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Market.Aggregates;
using MarketBook.Domain.Market.ValueObjects;
using MarketBook.Domain.Venue.Aggregates;
using MarketBook.Domain.Venue.Enums;
using MarketBook.Domain.Venue.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Venues.Commands.CreateVenue;

/// <summary>
/// EN: Handles the command used to create a new trading venue.
/// FA: فرمان ایجاد یک بستر معاملاتی جدید را پردازش می‌کند.
/// </summary>
public sealed class CreateVenueCommandHandler
    : IRequestHandler<CreateVenueCommand, Result<VenueId>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMarketRepository _marketRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="CreateVenueCommandHandler"/> class.
    /// FA: یک نمونه جدید از کلاس <see cref="CreateVenueCommandHandler"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="venueRepository">
    /// EN: Trading venue repository.
    /// FA: مخزن بسترهای معاملاتی.
    /// </param>
    /// <param name="marketRepository">
    /// EN: Market repository.
    /// FA: مخزن بازارها.
    /// </param>
    /// <param name="dbContext">
    /// EN: Application database context abstraction.
    /// FA: انتزاع Context پایگاه داده برنامه.
    /// </param>
    public CreateVenueCommandHandler(
        IVenueRepository venueRepository,
        IMarketRepository marketRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(venueRepository);
        ArgumentNullException.ThrowIfNull(marketRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _venueRepository = venueRepository;
        _marketRepository = marketRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Creates a new trading venue.
    /// FA: یک بستر معاملاتی جدید ایجاد می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Venue creation command.
    /// FA: فرمان ایجاد بستر معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The identifier of the created venue or a domain/application error.
    /// FA: شناسه بستر معاملاتی ایجادشده یا خطای دامنه/برنامه.
    /// </returns>
    public async Task<Result<VenueId>> Handle(
        CreateVenueCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!MarketId.TryParse(
                request.MarketId,
                out MarketId? marketId) ||
            marketId is null)
        {
            return Result<VenueId>.Fail(
                new Error(
                    "Venue.InvalidMarketId",
                    "The specified market identifier is invalid."));
        }

        Market? market =
            await _marketRepository.GetByIdAsync(
                marketId,
                cancellationToken);

        if (market is null)
        {
            return Result<VenueId>.Fail(
                new Error(
                    "Venue.MarketNotFound",
                    "The specified market was not found."));
        }

        if (!market.IsActive)
        {
            return Result<VenueId>.Fail(
                new Error(
                    "Venue.MarketInactive",
                    "The specified market is inactive."));
        }

        VenueCode code;

        try
        {
            code = new VenueCode(request.Code);
        }
        catch (ArgumentException)
        {
            return Result<VenueId>.Fail(
                new Error(
                    "Venue.InvalidCode",
                    "The specified venue code is invalid."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<VenueId>.Fail(
                new Error(
                    "Venue.InvalidName",
                    "Venue name is required."));
        }

        if (!Enum.IsDefined(typeof(VenueType), request.Type))
        {
            return Result<VenueId>.Fail(
                new Error(
                    "Venue.InvalidType",
                    "The specified venue type is invalid."));
        }

        if (await _venueRepository.ExistsAsync(
                code,
                cancellationToken))
        {
            return Result<VenueId>.Fail(
                new Error(
                    "Venue.DuplicateCode",
                    "A venue with the specified code already exists."));
        }

        Venue venue = Venue.Create(
            marketId,
            code,
            request.Name,
            (VenueType)request.Type);

        await _venueRepository.AddAsync(
            venue,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result<VenueId>.Success(
            venue.Id);
    }
}
