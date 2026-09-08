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
using MarketBook.Application.Features.PortfolioCashTransactions.Commands.CreatePortfolioCashTransaction;
using MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashBalances;
using MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactionById;
using MarketBook.Application.Features.PortfolioCashTransactions.Queries.GetPortfolioCashTransactions;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>EN: Provides immutable portfolio cash-ledger endpoints. FA: Endpointهای دفتر نقدی تغییرناپذیر پرتفوی را فراهم می‌کند.</summary>
[ApiController]
[Route("api/v1/portfolio-cash-transactions")]
public sealed class PortfolioCashTransactionsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>EN: Initializes the controller. FA: Controller را مقداردهی اولیه می‌کند.</summary>
    public PortfolioCashTransactionsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>EN: Appends one cash-ledger transaction. FA: یک تراکنش به دفتر نقدی اضافه می‌کند.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePortfolioCashTransactionRequest request, CancellationToken cancellationToken)
    {
        CreatePortfolioCashTransactionCommand command = new(
            request.PortfolioId, request.CurrencyId, request.Type, request.Amount, request.OccurredOn,
            request.ReferenceType, request.ReferenceId, request.Description);
        Result<PortfolioCashTransactionId> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Value.ToString() }, new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>EN: Gets one cash-ledger transaction. FA: یک تراکنش دفتر نقدی را دریافت می‌کند.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        Result<GetPortfolioCashTransactionByIdResponse> result = await _sender.Send(
            new GetPortfolioCashTransactionByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>EN: Gets paged cash-ledger entries, optionally filtered by currency. FA: سطرهای صفحه‌بندی‌شده دفتر نقدی را با فیلتر اختیاری ارز دریافت می‌کند.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string portfolioId,
        [FromQuery] string? currencyId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<GetPortfolioCashTransactionsResponse> result = await _sender.Send(
            new GetPortfolioCashTransactionsQuery(portfolioId, currencyId, page, pageSize), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>EN: Projects multi-currency cash balances from the immutable ledger. FA: موجودی نقدی چندارزی را از دفتر تغییرناپذیر محاسبه می‌کند.</summary>
    [HttpGet("balances")]
    public async Task<IActionResult> GetBalances([FromQuery] string portfolioId, CancellationToken cancellationToken = default)
    {
        Result<GetPortfolioCashBalancesResponse> result = await _sender.Send(
            new GetPortfolioCashBalancesQuery(portfolioId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ApiErrorMapper.ToActionResult(this, result.Error);
    }
}
