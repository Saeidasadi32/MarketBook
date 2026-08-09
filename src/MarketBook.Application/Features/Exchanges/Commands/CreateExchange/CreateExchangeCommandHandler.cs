// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Commands.CreateExchange
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Country.Aggregates;
using MarketBook.Domain.Country.ValueObjects;
using MarketBook.Domain.Exchange.Aggregates;
using MarketBook.Domain.Exchange.ValueObjects;

namespace MarketBook.Application.Features.Exchanges.Commands.CreateExchange;

/// <summary>
/// EN: Handles the CreateExchangeCommand.
/// FA: فرمان ایجاد بورس را پردازش می‌کند.
/// </summary>
public sealed class CreateExchangeCommandHandler
    : ICommandHandler<CreateExchangeCommand, Result<ExchangeId>>
{
    private readonly IExchangeRepository _exchangeRepository;
    private readonly ICountryRepository _countryRepository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exchangeRepository"></param>
    /// <param name="countryRepository"></param>
    public CreateExchangeCommandHandler(
        IExchangeRepository exchangeRepository,
        ICountryRepository countryRepository)
    {
        ArgumentNullException.ThrowIfNull(exchangeRepository);
        ArgumentNullException.ThrowIfNull(countryRepository);

        _exchangeRepository = exchangeRepository;
        _countryRepository = countryRepository;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<ExchangeId>> Handle(
        CreateExchangeCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!CountryId.TryParse(
                request.CountryId,
                out CountryId? countryId) ||
            countryId is null)
        {
            return Result<ExchangeId>.Fail(
                new Error(
                    "Exchange.InvalidCountryId",
                    "The specified country identifier is invalid."));
        }

        ExchangeCode code;

        try
        {
            code = new ExchangeCode(request.Code);
        }
        catch (ArgumentException)
        {
            return Result<ExchangeId>.Fail(
                new Error(
                    "Exchange.InvalidCode",
                    "The specified exchange code is invalid."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<ExchangeId>.Fail(
                new Error(
                    "Exchange.InvalidName",
                    "Exchange name is required."));
        }

        Country? country = await _countryRepository.GetByIdAsync(
            countryId,
            cancellationToken);

        if (country is null)
        {
            return Result<ExchangeId>.Fail(
                new Error(
                    "Exchange.CountryNotFound",
                    "The specified country was not found."));
        }

        if (await _exchangeRepository.ExistsAsync(
                code,
                cancellationToken))
        {
            return Result<ExchangeId>.Fail(
                new Error(
                    "Exchange.DuplicateCode",
                    "An exchange with the specified code already exists."));
        }

        Exchange exchange = new(
            ExchangeId.New(),
            countryId,
            code,
            request.Name);

        await _exchangeRepository.AddAsync(
            exchange,
            cancellationToken);

        return Result<ExchangeId>.Success(exchange.Id);
    }
}
