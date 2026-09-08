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
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;

namespace MarketBook.Domain.Portfolio.Aggregates;

/// <summary>
/// EN: Represents one immutable cash-ledger entry for a portfolio and currency.
/// FA: یک سطر تغییرناپذیر دفتر نقدی برای یک پرتفوی و ارز را نمایش می‌دهد.
/// </summary>
public sealed class PortfolioCashTransaction : AggregateRoot<PortfolioCashTransactionId>
{
    private PortfolioCashTransaction()
    {
        PortfolioId = default!;
        CurrencyId = default!;
    }

    private PortfolioCashTransaction(
        PortfolioCashTransactionId id,
        PortfolioId portfolioId,
        CurrencyId currencyId,
        PortfolioCashTransactionType type,
        decimal amount,
        DateTimeOffset occurredOn,
        string? referenceType,
        string? referenceId,
        string? description)
        : base(id)
    {
        PortfolioId = portfolioId;
        CurrencyId = currencyId;
        Type = type;
        Amount = amount;
        OccurredOn = occurredOn;
        ReferenceType = NormalizeOptional(referenceType);
        ReferenceId = NormalizeOptional(referenceId);
        Description = NormalizeOptional(description);
        CreatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>EN: Portfolio identifier. FA: شناسه پرتفوی.</summary>
    public PortfolioId PortfolioId { get; private set; }
    /// <summary>EN: Currency identifier. FA: شناسه ارز.</summary>
    public CurrencyId CurrencyId { get; private set; }
    /// <summary>EN: Cash transaction type. FA: نوع تراکنش نقدی.</summary>
    public PortfolioCashTransactionType Type { get; private set; }
    /// <summary>EN: Positive absolute cash amount. FA: مبلغ مطلق و مثبت تراکنش نقدی.</summary>
    public decimal Amount { get; private set; }
    /// <summary>EN: Business occurrence timestamp. FA: زمان وقوع تجاری تراکنش.</summary>
    public DateTimeOffset OccurredOn { get; private set; }
    /// <summary>EN: Optional source/reference category. FA: نوع اختیاری مرجع یا منبع.</summary>
    public string? ReferenceType { get; private set; }
    /// <summary>EN: Optional source/reference identifier. FA: شناسه اختیاری مرجع یا منبع.</summary>
    public string? ReferenceId { get; private set; }
    /// <summary>EN: Optional human-readable description. FA: توضیح اختیاری قابل‌خواندن.</summary>
    public string? Description { get; private set; }
    /// <summary>EN: Persistence creation timestamp. FA: زمان ایجاد رکورد ماندگار.</summary>
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>
    /// EN: Gets whether this entry increases cash balance.
    /// FA: مشخص می‌کند این سطر موجودی نقدی را افزایش می‌دهد یا خیر.
    /// </summary>
    public bool IsCredit => Type is
        PortfolioCashTransactionType.Deposit or
        PortfolioCashTransactionType.SellSettlement or
        PortfolioCashTransactionType.Dividend or
        PortfolioCashTransactionType.Interest or
        PortfolioCashTransactionType.OtherCredit;

    /// <summary>
    /// EN: Returns signed balance impact while the stored amount remains positive.
    /// FA: اثر علامت‌دار بر موجودی را برمی‌گرداند، در حالی که مبلغ ذخیره‌شده مثبت باقی می‌ماند.
    /// </summary>
    public decimal SignedAmount => IsCredit ? Amount : -Amount;

    /// <summary>
    /// EN: Creates an immutable cash-ledger transaction.
    /// FA: یک تراکنش تغییرناپذیر دفتر نقدی ایجاد می‌کند.
    /// </summary>
    public static PortfolioCashTransaction Create(
        PortfolioId portfolioId,
        CurrencyId currencyId,
        PortfolioCashTransactionType type,
        decimal amount,
        DateTimeOffset occurredOn,
        string? referenceType = null,
        string? referenceId = null,
        string? description = null)
    {
        ArgumentNullException.ThrowIfNull(portfolioId);
        ArgumentNullException.ThrowIfNull(currencyId);

        if (!Enum.IsDefined(typeof(PortfolioCashTransactionType), type))
            throw new DomainException(new Error(
                "PortfolioCashTransaction.InvalidType",
                "The cash transaction type is invalid."));

        if (amount <= 0m)
            throw new DomainException(new Error(
                "PortfolioCashTransaction.InvalidAmount",
                "Cash transaction amount must be greater than zero."));

        if (referenceType?.Trim().Length > 64)
            throw new DomainException(new Error(
                "PortfolioCashTransaction.ReferenceTypeTooLong",
                "Reference type cannot exceed 64 characters."));

        if (referenceId?.Trim().Length > 128)
            throw new DomainException(new Error(
                "PortfolioCashTransaction.ReferenceIdTooLong",
                "Reference identifier cannot exceed 128 characters."));

        if (description?.Trim().Length > 500)
            throw new DomainException(new Error(
                "PortfolioCashTransaction.DescriptionTooLong",
                "Description cannot exceed 500 characters."));

        return new PortfolioCashTransaction(
            PortfolioCashTransactionId.New(),
            portfolioId,
            currencyId,
            type,
            amount,
            occurredOn,
            referenceType,
            referenceId,
            description);
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
