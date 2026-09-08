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
using MarketBook.Application.Features.Investors.Commands.ActivateInvestor;
using MarketBook.Application.Features.Investors.Commands.CreateInvestor;
using MarketBook.Application.Features.Investors.Commands.DeactivateInvestor;
using MarketBook.Application.Features.Investors.Commands.UpdateInvestor;
using MarketBook.Application.Features.Investors.Queries.GetAllInvestors;
using MarketBook.Application.Features.Investors.Queries.GetInvestorById;
using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>EN: Provides Investor HTTP endpoints. FA: Endpointهای HTTP سرمایه‌گذار را فراهم می‌کند.</summary>
[ApiController]
[Route("api/v1/investors")]
public sealed class InvestorsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>EN: Initializes the controller. FA: Controller را مقداردهی می‌کند.</summary>
    public InvestorsController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>EN: Creates an investor. FA: سرمایه‌گذار ایجاد می‌کند.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateInvestorRequest request,
        CancellationToken cancellationToken)
    {
        Result<InvestorId> result = await _sender.Send(
            new CreateInvestorCommand(request.FullName),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Value.ToString() },
                new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>EN: Updates an investor. FA: سرمایه‌گذار را به‌روزرسانی می‌کند.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateInvestorRequest request,
        CancellationToken cancellationToken)
    {
        Result<InvestorId> result = await _sender.Send(
            new UpdateInvestorCommand(id, request.FullName),
            cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>EN: Activates an investor. FA: سرمایه‌گذار را فعال می‌کند.</summary>
    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(
        string id,
        CancellationToken cancellationToken)
    {
        Result<InvestorId> result =
            await _sender.Send(new ActivateInvestorCommand(id), cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>EN: Deactivates an investor. FA: سرمایه‌گذار را غیرفعال می‌کند.</summary>
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(
        string id,
        CancellationToken cancellationToken)
    {
        Result<InvestorId> result =
            await _sender.Send(new DeactivateInvestorCommand(id), cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>EN: Gets an investor by ID. FA: سرمایه‌گذار را با شناسه دریافت می‌کند.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetInvestorByIdResponse> result =
            await _sender.Send(new GetInvestorByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>EN: Gets paged investors. FA: سرمایه‌گذاران صفحه‌بندی‌شده را دریافت می‌کند.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<GetAllInvestorsResponse> result =
            await _sender.Send(new GetAllInvestorsQuery(page, pageSize), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }
}
