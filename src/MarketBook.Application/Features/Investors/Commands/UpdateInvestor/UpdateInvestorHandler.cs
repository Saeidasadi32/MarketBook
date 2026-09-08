// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Commands.UpdateInvestor
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.Aggregates;
using MarketBook.Domain.Investor.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Investors.Commands.UpdateInvestor;

/// <summary>EN: Handles investor updates. FA: به‌روزرسانی سرمایه‌گذار را مدیریت می‌کند.</summary>
public sealed class UpdateInvestorHandler
    : IRequestHandler<UpdateInvestorCommand, Result<InvestorId>>
{
    private readonly IInvestorRepository _repository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public UpdateInvestorHandler(
        IInvestorRepository repository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(dbContext);
        _repository = repository;
        _dbContext = dbContext;
    }

    /// <summary>EN: Handles investor update. FA: به‌روزرسانی سرمایه‌گذار را پردازش می‌کند.</summary>
    public async Task<Result<InvestorId>> Handle(
        UpdateInvestorCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!InvestorId.TryParse(request.Id, out InvestorId? investorId) || investorId is null)
        {
            return Result<InvestorId>.Fail(
                new Error("Investor.InvalidId", "The investor identifier is invalid."));
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return Result<InvestorId>.Fail(
                new Error("Investor.InvalidName", "Investor full name is required."));
        }

        Investor? investor = await _repository.GetByIdAsync(investorId, cancellationToken);

        if (investor is null)
        {
            return Result<InvestorId>.Fail(
                new Error("Investor.NotFound", "The investor was not found."));
        }

        investor.Rename(request.FullName);
        _repository.Update(investor);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<InvestorId>.Success(investor.Id);
    }
}
