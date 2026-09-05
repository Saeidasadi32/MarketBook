// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Commands.CreateCurrency
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Currencies.Commands.CreateCurrency;

/// <summary>
/// EN: Handles creating a Currency aggregate.
/// FA: ایجاد Aggregate ارز را مدیریت می‌کند.
/// </summary>
public sealed class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, Result<CurrencyId>>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the create currency handler.
    /// FA: Handler ایجاد ارز را مقداردهی می‌کند.
    /// </summary>
    public CreateCurrencyCommandHandler(ICurrencyRepository currencyRepository, IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(currencyRepository);
        ArgumentNullException.ThrowIfNull(dbContext);
        _currencyRepository = currencyRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the create currency command.
    /// FA: فرمان ایجاد ارز را پردازش می‌کند.
    /// </summary>
    public async Task<Result<CurrencyId>> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        CurrencyCode code;
        try { code = new CurrencyCode(request.Code); }
        catch (ArgumentException)
        {
            return Result<CurrencyId>.Fail(new Error("Currency.InvalidCode", "The specified currency code is invalid."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<CurrencyId>.Fail(new Error("Currency.InvalidName", "Currency name is required."));

        if (request.DecimalPlaces < 0 || request.DecimalPlaces > 18)
            return Result<CurrencyId>.Fail(new Error("Currency.InvalidDecimalPlaces", "Decimal places must be between 0 and 18."));

        if (await _currencyRepository.ExistsAsync(code, cancellationToken))
            return Result<CurrencyId>.Fail(new Error("Currency.DuplicateCode", "A currency with the specified code already exists."));

        Currency currency = Currency.Create(code, request.Name, (byte)request.DecimalPlaces);
        await _currencyRepository.AddAsync(currency, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result<CurrencyId>.Success(currency.Id);
    }
}
