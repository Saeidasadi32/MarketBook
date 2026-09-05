// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Commands.DeactivateCurrency
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MediatR;
namespace MarketBook.Application.Features.Currencies.Commands.DeactivateCurrency;
/// <summary>EN: Handles deactivateing a Currency aggregate. FA: غیرفعال‌سازی Aggregate ارز را مدیریت می‌کند.</summary>
public sealed class DeactivateCurrencyHandler : IRequestHandler<DeactivateCurrencyCommand, Result<CurrencyId>>
{
 private readonly ICurrencyRepository _currencyRepository; private readonly IApplicationDbContext _dbContext;
 /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
 public DeactivateCurrencyHandler(ICurrencyRepository currencyRepository,IApplicationDbContext dbContext){ArgumentNullException.ThrowIfNull(currencyRepository);ArgumentNullException.ThrowIfNull(dbContext);_currencyRepository=currencyRepository;_dbContext=dbContext;}
 /// <summary>EN: Handles the command. FA: فرمان را پردازش می‌کند.</summary>
 public async Task<Result<CurrencyId>> Handle(DeactivateCurrencyCommand request,CancellationToken cancellationToken){ArgumentNullException.ThrowIfNull(request);if(!CurrencyId.TryParse(request.Id,out CurrencyId? id)||id is null)return Result<CurrencyId>.Fail(new Error("Currency.InvalidId","The specified currency identifier is invalid."));Currency? currency=await _currencyRepository.GetByIdAsync(id,cancellationToken);if(currency is null)return Result<CurrencyId>.Fail(new Error("Currency.NotFound","The specified currency was not found."));currency.Deactivate();_currencyRepository.Update(currency);await _dbContext.SaveChangesAsync(cancellationToken);return Result<CurrencyId>.Success(currency.Id);}
}
