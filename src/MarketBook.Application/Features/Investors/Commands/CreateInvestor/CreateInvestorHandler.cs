// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Investors.Commands.CreateInvestor
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Investor.Aggregates;
using MarketBook.Domain.Investor.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Investors.Commands.CreateInvestor;

/// <summary>EN: Handles investor creation. FA: ایجاد سرمایه‌گذار را مدیریت می‌کند.</summary>
public sealed class CreateInvestorHandler
    : IRequestHandler<CreateInvestorCommand, Result<InvestorId>>
{
    private readonly IInvestorRepository _repository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public CreateInvestorHandler(
        IInvestorRepository repository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(dbContext);
        _repository = repository;
        _dbContext = dbContext;
    }

    /// <summary>EN: Handles investor creation. FA: ایجاد سرمایه‌گذار را پردازش می‌کند.</summary>
    public async Task<Result<InvestorId>> Handle(
        CreateInvestorCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return Result<InvestorId>.Fail(
                new Error("Investor.InvalidName", "Investor full name is required."));
        }

        Investor investor = Investor.Create(request.FullName);
        await _repository.AddAsync(investor, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<InvestorId>.Success(investor.Id);
    }
}
