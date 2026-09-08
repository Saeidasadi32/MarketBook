// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioCashTransactions.Commands.CreatePortfolioCashTransaction
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioCashTransactions.Commands.CreatePortfolioCashTransaction;

/// <summary>
/// EN: Validates dependencies and appends one immutable cash-ledger entry.
/// FA: وابستگی‌ها را اعتبارسنجی و یک سطر تغییرناپذیر به دفتر نقدی اضافه می‌کند.
/// </summary>
public sealed class CreatePortfolioCashTransactionHandler
    : IRequestHandler<CreatePortfolioCashTransactionCommand, Result<PortfolioCashTransactionId>>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IPortfolioCashTransactionRepository _cashRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی اولیه می‌کند.</summary>
    public CreatePortfolioCashTransactionHandler(
        IPortfolioRepository portfolioRepository,
        ICurrencyRepository currencyRepository,
        IPortfolioCashTransactionRepository cashRepository,
        IApplicationDbContext dbContext)
    {
        _portfolioRepository = portfolioRepository;
        _currencyRepository = currencyRepository;
        _cashRepository = cashRepository;
        _dbContext = dbContext;
    }

    /// <summary>EN: Handles append-only cash transaction creation. FA: ایجاد تراکنش append-only دفتر نقدی را مدیریت می‌کند.</summary>
    public async Task<Result<PortfolioCashTransactionId>> Handle(
        CreatePortfolioCashTransactionCommand request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) || portfolioId is null)
            return Result<PortfolioCashTransactionId>.Fail(new Error(
                "PortfolioCashTransaction.InvalidPortfolioId",
                "The portfolio identifier is invalid."));

        if (!CurrencyId.TryParse(request.CurrencyId, out CurrencyId? currencyId) || currencyId is null)
            return Result<PortfolioCashTransactionId>.Fail(new Error(
                "PortfolioCashTransaction.InvalidCurrencyId",
                "The currency identifier is invalid."));

        Portfolio? portfolio = await _portfolioRepository.GetByIdAsync(portfolioId, cancellationToken);
        if (portfolio is null)
            return Result<PortfolioCashTransactionId>.Fail(new Error(
                "PortfolioCashTransaction.PortfolioNotFound",
                "The portfolio was not found."));

        if (!portfolio.IsActive)
            return Result<PortfolioCashTransactionId>.Fail(new Error(
                "PortfolioCashTransaction.PortfolioInactive",
                "The portfolio is inactive."));

        Currency? currency = await _currencyRepository.GetByIdAsync(currencyId, cancellationToken);
        if (currency is null)
            return Result<PortfolioCashTransactionId>.Fail(new Error(
                "PortfolioCashTransaction.CurrencyNotFound",
                "The currency was not found."));

        if (!currency.IsActive)
            return Result<PortfolioCashTransactionId>.Fail(new Error(
                "PortfolioCashTransaction.CurrencyInactive",
                "The currency is inactive."));

        if (!Enum.IsDefined(typeof(PortfolioCashTransactionType), request.Type))
            return Result<PortfolioCashTransactionId>.Fail(new Error(
                "PortfolioCashTransaction.InvalidType",
                "The cash transaction type is invalid."));

        PortfolioCashTransaction transaction;
        try
        {
            transaction = PortfolioCashTransaction.Create(
                portfolioId,
                currencyId,
                (PortfolioCashTransactionType)request.Type,
                request.Amount,
                request.OccurredOn,
                request.ReferenceType,
                request.ReferenceId,
                request.Description);
        }
        catch (DomainException exception)
        {
            return Result<PortfolioCashTransactionId>.Fail(exception.Error);
        }

        await _cashRepository.AddAsync(transaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result<PortfolioCashTransactionId>.Success(transaction.Id);
    }
}
