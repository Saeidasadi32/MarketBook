// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Commands.UpdateCurrency
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MediatR;
namespace MarketBook.Application.Features.Currencies.Commands.UpdateCurrency;
/// <summary>EN: Handles updating a Currency aggregate. FA: به‌روزرسانی Aggregate ارز را مدیریت می‌کند.</summary>
public sealed class UpdateCurrencyHandler : IRequestHandler<UpdateCurrencyCommand, Result<CurrencyId>>
{
    private readonly ICurrencyRepository _currencyRepository; private readonly IApplicationDbContext _dbContext;
    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public UpdateCurrencyHandler(ICurrencyRepository currencyRepository, IApplicationDbContext dbContext){ArgumentNullException.ThrowIfNull(currencyRepository);ArgumentNullException.ThrowIfNull(dbContext);_currencyRepository=currencyRepository;_dbContext=dbContext;}
    /// <summary>EN: Handles the update command. FA: فرمان به‌روزرسانی را پردازش می‌کند.</summary>
    public async Task<Result<CurrencyId>> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if(!CurrencyId.TryParse(request.Id,out CurrencyId? id)||id is null) return Result<CurrencyId>.Fail(new Error("Currency.InvalidId","The specified currency identifier is invalid."));
        Currency? currency=await _currencyRepository.GetByIdAsync(id,cancellationToken);
        if(currency is null) return Result<CurrencyId>.Fail(new Error("Currency.NotFound","The specified currency was not found."));
        CurrencyCode code; try{code=new CurrencyCode(request.Code);}catch(ArgumentException){return Result<CurrencyId>.Fail(new Error("Currency.InvalidCode","The specified currency code is invalid."));}
        if(string.IsNullOrWhiteSpace(request.Name)) return Result<CurrencyId>.Fail(new Error("Currency.InvalidName","Currency name is required."));
        if(request.DecimalPlaces<0||request.DecimalPlaces>18) return Result<CurrencyId>.Fail(new Error("Currency.InvalidDecimalPlaces","Decimal places must be between 0 and 18."));
        if(await _currencyRepository.ExistsAsync(code,id,cancellationToken)) return Result<CurrencyId>.Fail(new Error("Currency.DuplicateCode","A currency with the specified code already exists."));
        currency.ChangeCode(code); currency.Rename(request.Name); currency.ChangeDecimalPlaces((byte)request.DecimalPlaces);
        _currencyRepository.Update(currency); await _dbContext.SaveChangesAsync(cancellationToken); return Result<CurrencyId>.Success(currency.Id);
    }
}
