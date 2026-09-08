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
using MarketBook.Application.Features.PortfolioTransactions.Commands.CreatePortfolioTransaction;
using MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioPositions;
using MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioRealizedPnl;
using MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactionById;
using MarketBook.Application.Features.PortfolioTransactions.Queries.GetPortfolioTransactions;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides immutable portfolio transaction ledger endpoints.
/// FA: Endpointهای دفتر تراکنش تغییرناپذیر پرتفوی را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/portfolio-transactions")]
public sealed class PortfolioTransactionsController : ControllerBase
{
    private readonly ISender _sender;

    public PortfolioTransactionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePortfolioTransactionRequest request,
        CancellationToken cancellationToken)
    {
        CreatePortfolioTransactionCommand command = new(
            request.PortfolioId,
            request.ListingId,
            request.Type,
            request.Quantity,
            request.Price,
            request.Commission,
            request.Tax,
            request.ExchangeFee,
            request.BrokerFee,
            request.ClearingFee,
            request.OtherFees,
            request.ExecutedOn);

        Result<PortfolioTransactionId> result =
            await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Value.ToString() },
                new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioTransactionByIdResponse> result =
            await _sender.Send(
                new GetPortfolioTransactionByIdQuery(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string portfolioId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<GetPortfolioTransactionsResponse> result =
            await _sender.Send(
                new GetPortfolioTransactionsQuery(portfolioId, page, pageSize),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    [HttpGet("positions")]
    public async Task<IActionResult> GetPositions(
        [FromQuery] string portfolioId,
        CancellationToken cancellationToken = default)
    {
        Result<GetPortfolioPositionsResponse> result =
            await _sender.Send(
                new GetPortfolioPositionsQuery(portfolioId),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Reconstructs realized portfolio profit/loss from the immutable trade ledger.
    /// FA: سود/زیان تحقق‌یافته پرتفوی را از دفتر تغییرناپذیر معاملات بازسازی می‌کند.
    /// </summary>
    [HttpGet("realized-pnl")]
    public async Task<IActionResult> GetRealizedPnl(
        [FromQuery] string portfolioId,
        [FromQuery] string? listingId = null,
        [FromQuery] int costBasisMethod = 1,
        CancellationToken cancellationToken = default)
    {
        Result<GetPortfolioRealizedPnlResponse> result =
            await _sender.Send(
                new GetPortfolioRealizedPnlQuery(
                    portfolioId,
                    listingId,
                    costBasisMethod),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }
}
