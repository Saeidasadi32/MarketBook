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
using MarketBook.Application.Features.Portfolios.Commands.SetPortfolioBaseCurrency;
using MarketBook.Application.Features.Portfolios.Commands.UpdatePortfolio;
using MarketBook.Application.Features.Portfolios.Queries.GetAllPortfolios;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioById;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNav;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNavAsOf;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNav;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformance;
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
    /// EN: Sets the reporting/base currency of a portfolio.
    /// FA: ارز پایه/گزارش‌دهی یک پرتفوی را تنظیم می‌کند.
    /// </summary>
    [HttpPatch("{id}/base-currency")]
    public async Task<IActionResult> SetBaseCurrency(
        string id,
        [FromBody] SetPortfolioBaseCurrencyRequest request,
        CancellationToken cancellationToken)
    {
        Result<PortfolioId> result =
            await _sender.Send(
                new SetPortfolioBaseCurrencyCommand(
                    id,
                    request.CurrencyId),
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
    /// EN: Gets current net asset value grouped by currency.
    /// FA: ارزش خالص دارایی جاری را به تفکیک ارز دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/nav")]
    public async Task<IActionResult> GetNav(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioNavResponse> result =
            await _sender.Send(
                new GetPortfolioNavQuery(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets portfolio NAV reconstructed at an inclusive historical cutoff instant.
    /// FA: NAV پرتفوی را در یک لحظه تاریخی شامل‌شونده بازسازی و دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/nav/as-of")]
    public async Task<IActionResult> GetNavAsOf(
        string id,
        [FromQuery] DateTimeOffset asOf,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioNavAsOfResponse> result =
            await _sender.Send(
                new GetPortfolioNavAsOfQuery(id, asOf),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets current portfolio NAV translated into the configured base currency.
    /// FA: NAV جاری پرتفوی را پس از ترجمه به ارز پایه تنظیم‌شده دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/translated-nav")]
    public async Task<IActionResult> GetTranslatedNav(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioTranslatedNavResponse> result =
            await _sender.Send(
                new GetPortfolioTranslatedNavQuery(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets historical portfolio NAV translated into the configured base currency using FX available on or before the as-of date.
    /// FA: NAV تاریخی پرتفوی را با استفاده از نرخ FX موجود در تاریخ As-Of یا قبل از آن به ارز پایه تنظیم‌شده ترجمه می‌کند.
    /// </summary>
    [HttpGet("{id}/translated-nav/as-of")]
    public async Task<IActionResult> GetTranslatedNavAsOf(
        string id,
        [FromQuery] DateTimeOffset asOf,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioTranslatedNavAsOfResponse> result =
            await _sender.Send(
                new GetPortfolioTranslatedNavAsOfQuery(id, asOf),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets cash-flow-aware historical portfolio performance foundation in the configured base currency.
    /// FA: مبنای عملکرد تاریخی پرتفوی را با لحاظ جریان نقدی خارجی و در ارز پایه تنظیم‌شده دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/performance")]
    public async Task<IActionResult> GetPerformance(
        string id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioPerformanceResponse> result =
            await _sender.Send(
                new GetPortfolioPerformanceQuery(id, from, to),
                cancellationToken);

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
