// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Queries.GetInvestorById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.Aggregates;
using MarketBook.Domain.Investor.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Investors.Queries.GetInvestorById;

/// <summary>EN: Handles investor lookup by ID. FA: دریافت سرمایه‌گذار با شناسه را مدیریت می‌کند.</summary>
public sealed class GetInvestorByIdHandler
    : IRequestHandler<GetInvestorByIdQuery, Result<GetInvestorByIdResponse>>
{
    private readonly IInvestorRepository _repository;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public GetInvestorByIdHandler(IInvestorRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>EN: Handles the query. FA: پرس‌وجو را پردازش می‌کند.</summary>
    public async Task<Result<GetInvestorByIdResponse>> Handle(
        GetInvestorByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!InvestorId.TryParse(request.Id, out InvestorId? investorId) || investorId is null)
        {
            return Result<GetInvestorByIdResponse>.Fail(
                new Error("Investor.InvalidId", "The investor identifier is invalid."));
        }

        Investor? investor = await _repository.GetByIdAsync(investorId, cancellationToken);

        if (investor is null)
        {
            return Result<GetInvestorByIdResponse>.Fail(
                new Error("Investor.NotFound", "The investor was not found."));
        }

        GetInvestorByIdResponse response = new(
            investor.Id.Value.ToString(),
            investor.FullName,
            investor.CreatedOn,
            investor.IsActive);

        return Result<GetInvestorByIdResponse>.Success(response);
    }
}
