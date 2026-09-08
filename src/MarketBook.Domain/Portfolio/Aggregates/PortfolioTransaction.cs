// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Portfolio.Aggregates
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;

namespace MarketBook.Domain.Portfolio.Aggregates;

/// <summary>
/// EN: Represents an immutable executed Buy/Sell transaction in a portfolio ledger.
/// FA: یک تراکنش اجراشده و تغییرناپذیر خرید/فروش در دفتر پرتفوی را نمایش می‌دهد.
/// </summary>
public sealed class PortfolioTransaction : AggregateRoot<PortfolioTransactionId>
{
    private PortfolioTransaction()
    {
        PortfolioId = default!;
        ListingId = default!;
        CurrencyId = default!;
    }

    private PortfolioTransaction(
        PortfolioTransactionId id,
        PortfolioId portfolioId,
        ListingId listingId,
        CurrencyId currencyId,
        PortfolioEventType type,
        decimal quantity,
        decimal price,
        decimal commission,
        decimal tax,
        decimal exchangeFee,
        decimal brokerFee,
        decimal clearingFee,
        decimal otherFees,
        DateTimeOffset executedOn)
        : base(id)
    {
        PortfolioId = portfolioId;
        ListingId = listingId;
        CurrencyId = currencyId;
        Type = type;
        Quantity = quantity;
        Price = price;
        Commission = commission;
        Tax = tax;
        ExchangeFee = exchangeFee;
        BrokerFee = brokerFee;
        ClearingFee = clearingFee;
        OtherFees = otherFees;
        ExecutedOn = executedOn;
        CreatedOn = DateTimeOffset.UtcNow;
    }

    public PortfolioId PortfolioId { get; private set; }
    public ListingId ListingId { get; private set; }
    public CurrencyId CurrencyId { get; private set; }
    public PortfolioEventType Type { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }
    public decimal Commission { get; private set; }
    public decimal Tax { get; private set; }
    public decimal ExchangeFee { get; private set; }
    public decimal BrokerFee { get; private set; }
    public decimal ClearingFee { get; private set; }
    public decimal OtherFees { get; private set; }
    public DateTimeOffset ExecutedOn { get; private set; }
    public DateTimeOffset CreatedOn { get; private set; }

    public decimal GrossValue => Quantity * Price;

    public decimal TotalCosts =>
        Commission + Tax + ExchangeFee + BrokerFee + ClearingFee + OtherFees;

    /// <summary>
    /// EN: Creates an immutable Buy/Sell ledger transaction.
    /// FA: یک تراکنش تغییرناپذیر خرید/فروش در دفتر پرتفوی ایجاد می‌کند.
    /// </summary>
    public static PortfolioTransaction Create(
        PortfolioId portfolioId,
        ListingId listingId,
        CurrencyId currencyId,
        PortfolioEventType type,
        decimal quantity,
        decimal price,
        decimal commission,
        decimal tax,
        decimal exchangeFee,
        decimal brokerFee,
        decimal clearingFee,
        decimal otherFees,
        DateTimeOffset executedOn)
    {
        ArgumentNullException.ThrowIfNull(portfolioId);
        ArgumentNullException.ThrowIfNull(listingId);
        ArgumentNullException.ThrowIfNull(currencyId);

        if (type != PortfolioEventType.Buy &&
            type != PortfolioEventType.Sell)
        {
            throw new DomainException(
                new Error(
                    "PortfolioTransaction.UnsupportedType",
                    "Only Buy and Sell transactions are supported in this slice."));
        }

        if (quantity <= 0m)
            throw new DomainException(
                new Error(
                    "PortfolioTransaction.InvalidQuantity",
                    "Transaction quantity must be greater than zero."));

        if (price <= 0m)
            throw new DomainException(
                new Error(
                    "PortfolioTransaction.InvalidPrice",
                    "Transaction price must be greater than zero."));

        if (commission < 0m ||
            tax < 0m ||
            exchangeFee < 0m ||
            brokerFee < 0m ||
            clearingFee < 0m ||
            otherFees < 0m)
        {
            throw new DomainException(
                new Error(
                    "PortfolioTransaction.InvalidCost",
                    "Transaction costs cannot be negative."));
        }

        return new PortfolioTransaction(
            PortfolioTransactionId.New(),
            portfolioId,
            listingId,
            currencyId,
            type,
            quantity,
            price,
            commission,
            tax,
            exchangeFee,
            brokerFee,
            clearingFee,
            otherFees,
            executedOn);
    }
}
