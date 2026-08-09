// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : API
// -----------------------------------------------------------------------------

using MarketBook.Application.Common.Pagination;
using MarketBook.Application.Features.Countries.Commands.CreateCountry;
using MarketBook.Application.Features.Countries.Queries.GetCountries;
using MarketBook.Application.Features.Countries.Queries.GetCountryById;
using MarketBook.Application.Features.Countries.Responses;
using MarketBook.Domain.Common;
using MarketBook.Domain.Country.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// Provides endpoints for country management.
/// </summary>
[ApiController]
[Route("api/v1/countries")]
public sealed class CountriesController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="CountriesController"/> class.
    /// </summary>
    /// <param name="sender">MediatR request sender.</param>
    public CountriesController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// Creates a new country.
    /// </summary>
    /// <param name="command">Country creation command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The identifier of the created country.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCountryCommand command,
        CancellationToken cancellationToken)
    {
        Result<CountryId> result = await _sender
            .Send(command, cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        string id = result.Value!.ToString();

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            new
            {
                id
            });
    }

    /// <summary>
    /// Gets a country by identifier.
    /// </summary>
    /// <param name="id">Country identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested country.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest();
        }

        GetCountryByIdQuery query = new(id);

        Result<CountryResponse> result = await _sender
            .Send(query, cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                "Country.InvalidId" => BadRequest(result.Error),
                "Country.NotFound" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets countries with pagination.
    /// </summary>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of countries.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        GetCountriesQuery query = new(
            page,
            pageSize);

        Result<PagedResult<CountryResponse>> result =
            await _sender
                .Send(query, cancellationToken)
                .ConfigureAwait(false);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
