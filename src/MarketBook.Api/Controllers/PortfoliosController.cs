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
using MarketBook.Application.Features.Portfolios.Commands.ActivatePortfolio;
using MarketBook.Application.Features.Portfolios.Commands.CreatePortfolio;
using MarketBook.Application.Features.Portfolios.Commands.DeactivatePortfolio;
using MarketBook.Application.Features.Portfolios.Commands.UpdatePortfolio;
using MarketBook.Application.Features.Portfolios.Queries.GetAllPortfolios;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioById;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides Portfolio HTTP endpoints.
/// FA: Endpointهای HTTP پرتفوی را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/portfolios")]
public sealed class PortfoliosController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the controller.
    /// FA: Controller را مقداردهی می‌کند.
    /// </summary>
    public PortfoliosController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Creates a portfolio.
    /// FA: یک پرتفوی ایجاد می‌کند.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePortfolioRequest request,
        CancellationToken cancellationToken)
    {
        Result<PortfolioId> result = await _sender.Send(
            new CreatePortfolioCommand(request.InvestorId, request.Name),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Value.ToString() },
                new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Renames a portfolio.
    /// FA: نام پرتفوی را تغییر می‌دهد.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdatePortfolioRequest request,
        CancellationToken cancellationToken)
    {
        Result<PortfolioId> result = await _sender.Send(
            new UpdatePortfolioCommand(id, request.Name),
            cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Activates a portfolio.
    /// FA: پرتفوی را فعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(
        string id,
        CancellationToken cancellationToken)
    {
        Result<PortfolioId> result =
            await _sender.Send(new ActivatePortfolioCommand(id), cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Deactivates a portfolio.
    /// FA: پرتفوی را غیرفعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(
        string id,
        CancellationToken cancellationToken)
    {
        Result<PortfolioId> result =
            await _sender.Send(new DeactivatePortfolioCommand(id), cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets a portfolio by identifier.
    /// FA: پرتفوی را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioByIdResponse> result =
            await _sender.Send(new GetPortfolioByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets paged portfolios.
    /// FA: پرتفوی‌های صفحه‌بندی‌شده را دریافت می‌کند.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? investorId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<GetAllPortfoliosResponse> result =
            await _sender.Send(
                new GetAllPortfoliosQuery(page, pageSize, investorId),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }
}
