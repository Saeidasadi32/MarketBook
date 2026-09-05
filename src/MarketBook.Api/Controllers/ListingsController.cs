// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : API
// Namespace : MarketBook.Api.Controllers
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Api.Common;
using MarketBook.Application.Features.Listings.Commands.ActivateListing;
using MarketBook.Application.Features.Listings.Commands.CreateListing;
using MarketBook.Application.Features.Listings.Commands.DeactivateListing;
using MarketBook.Application.Features.Listings.Commands.MakePrimaryListing;
using MarketBook.Application.Features.Listings.Commands.RemovePrimaryListing;
using MarketBook.Application.Features.Listings.Commands.UpdateListing;
using MarketBook.Application.Features.Listings.Queries.GetAllListings;
using MarketBook.Application.Features.Listings.Queries.GetListingById;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides HTTP endpoints for listing operations.
/// FA: نقاط پایانی HTTP مربوط به عملیات Listing را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/listings")]
public sealed class ListingsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the controller.
    /// FA: Controller را مقداردهی می‌کند.
    /// </summary>
    public ListingsController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Creates a listing.
    /// FA: Listing جدید ایجاد می‌کند.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateListingRequest request,
        CancellationToken cancellationToken)
    {
        CreateListingCommand command = new(
            request.InstrumentId,
            request.VenueId,
            request.QuoteCurrencyId,
            request.TradingSymbol,
            request.TickSize,
            request.PricePrecision);

        Result<ListingId> result =
            await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Value.ToString() },
                new { id = result.Value!.Value.ToString() });
        }

        return ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Updates mutable listing attributes.
    /// FA: ویژگی‌های قابل تغییر Listing را به‌روزرسانی می‌کند.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateListingRequest request,
        CancellationToken cancellationToken)
    {
        UpdateListingCommand command = new(
            id,
            request.QuoteCurrencyId,
            request.TradingSymbol,
            request.TickSize,
            request.PricePrecision);

        Result<ListingId> result =
            await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets a listing by identifier.
    /// FA: Listing را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetListingByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetListingByIdResponse> result =
            await _sender.Send(
                new GetListingByIdQuery(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets a paged collection of listings.
    /// FA: مجموعه صفحه‌بندی‌شده Listingها را دریافت می‌کند.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetAllListingsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<GetAllListingsResponse> result =
            await _sender.Send(
                new GetAllListingsQuery(page, pageSize),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Activates a listing.
    /// FA: Listing را فعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(
        string id,
        CancellationToken cancellationToken)
    {
        Result<ListingId> result =
            await _sender.Send(
                new ActivateListingCommand(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Deactivates a listing.
    /// FA: Listing را غیرفعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(
        string id,
        CancellationToken cancellationToken)
    {
        Result<ListingId> result =
            await _sender.Send(
                new DeactivateListingCommand(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Makes a listing primary for its instrument.
    /// FA: Listing را برای Instrument مربوطه به پذیرش اصلی تبدیل می‌کند.
    /// </summary>
    [HttpPatch("{id}/make-primary")]
    public async Task<IActionResult> MakePrimary(
        string id,
        CancellationToken cancellationToken)
    {
        Result<ListingId> result =
            await _sender.Send(
                new MakePrimaryListingCommand(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Removes the primary flag from a listing.
    /// FA: وضعیت اصلی را از Listing حذف می‌کند.
    /// </summary>
    [HttpPatch("{id}/remove-primary")]
    public async Task<IActionResult> RemovePrimary(
        string id,
        CancellationToken cancellationToken)
    {
        Result<ListingId> result =
            await _sender.Send(
                new RemovePrimaryListingCommand(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }
}
