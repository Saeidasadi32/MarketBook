// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioTransactions.Commands.CreatePortfolioTransaction
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioTransactions.Commands.CreatePortfolioTransaction;

/// <summary>
/// EN: Handles portfolio transaction creation.
/// FA: ایجاد تراکنش پرتفوی را مدیریت می‌کند.
/// </summary>
public sealed class CreatePortfolioTransactionHandler
    : IRequestHandler<CreatePortfolioTransactionCommand, Result<PortfolioTransactionId>>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IListingRepository _listingRepository;
    private readonly IPortfolioTransactionRepository _transactionRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public CreatePortfolioTransactionHandler(
        IPortfolioRepository portfolioRepository,
        IListingRepository listingRepository,
        IPortfolioTransactionRepository transactionRepository,
        IApplicationDbContext dbContext)
    {
        _portfolioRepository = portfolioRepository;
        _listingRepository = listingRepository;
        _transactionRepository = transactionRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Validates dependencies and appends one transaction to the ledger.
    /// FA: وابستگی‌ها را اعتبارسنجی و یک تراکنش را به دفتر اضافه می‌کند.
    /// </summary>
    public async Task<Result<PortfolioTransactionId>> Handle(
        CreatePortfolioTransactionCommand request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<PortfolioTransactionId>.Fail(
                new Error("PortfolioTransaction.InvalidPortfolioId", "The portfolio identifier is invalid."));
        }

        if (!ListingId.TryParse(request.ListingId, out ListingId? listingId) ||
            listingId is null)
        {
            return Result<PortfolioTransactionId>.Fail(
                new Error("PortfolioTransaction.InvalidListingId", "The listing identifier is invalid."));
        }

        Portfolio? portfolio = await _portfolioRepository.GetByIdAsync(portfolioId, cancellationToken);
        if (portfolio is null)
            return Result<PortfolioTransactionId>.Fail(
                new Error("PortfolioTransaction.PortfolioNotFound", "The portfolio was not found."));

        if (!portfolio.IsActive)
            return Result<PortfolioTransactionId>.Fail(
                new Error("PortfolioTransaction.PortfolioInactive", "The portfolio is inactive."));

        Listing? listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null)
            return Result<PortfolioTransactionId>.Fail(
                new Error("PortfolioTransaction.ListingNotFound", "The listing was not found."));

        if (!listing.IsActive)
            return Result<PortfolioTransactionId>.Fail(
                new Error("PortfolioTransaction.ListingInactive", "The listing is inactive."));

        if (!Enum.IsDefined(typeof(PortfolioEventType), request.Type))
            return Result<PortfolioTransactionId>.Fail(
                new Error("PortfolioTransaction.InvalidType", "The transaction type is invalid."));

        PortfolioEventType type = (PortfolioEventType)request.Type;

        if (type == PortfolioEventType.Sell)
        {
            List<PortfolioTransaction> ledger =
                await _transactionRepository.GetLedgerAsync(
                    portfolioId,
                    listingId,
                    cancellationToken);

            decimal available = ledger.Sum(item =>
                item.Type == PortfolioEventType.Buy
                    ? item.Quantity
                    : -item.Quantity);

            if (request.Quantity > available)
                return Result<PortfolioTransactionId>.Fail(
                    new Error(
                        "PortfolioTransaction.InsufficientQuantity",
                        "The sell quantity exceeds the current open quantity."));
        }

        PortfolioTransaction transaction;

        try
        {
            transaction = PortfolioTransaction.Create(
                portfolioId,
                listingId,
                listing.QuoteCurrencyId,
                type,
                request.Quantity,
                request.Price,
                request.Commission,
                request.Tax,
                request.ExchangeFee,
                request.BrokerFee,
                request.ClearingFee,
                request.OtherFees,
                request.ExecutedOn);
        }
        catch (DomainException exception)
        {
            return Result<PortfolioTransactionId>.Fail(exception.Error);
        }

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<PortfolioTransactionId>.Success(transaction.Id);
    }
}
