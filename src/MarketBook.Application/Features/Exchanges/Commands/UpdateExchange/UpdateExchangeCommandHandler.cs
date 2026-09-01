// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Commands.UpdateExchange
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Country.Aggregates;
using MarketBook.Domain.Country.ValueObjects;
using MarketBook.Domain.Exchange.Aggregates;
using MarketBook.Domain.Exchange.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Exchanges.Commands.UpdateExchange;

/// <summary>
/// EN: Handles exchange update requests.
/// FA: درخواست‌های به‌روزرسانی بورس را پردازش می‌کند.
/// </summary>
public sealed class UpdateExchangeCommandHandler
    : IRequestHandler<UpdateExchangeCommand, Result<ExchangeId>>
{
    private readonly IExchangeRepository _exchangeRepository;
    private readonly ICountryRepository _countryRepository;

    /// <summary>
    /// EN: Initializes a new instance of the handler.
    /// FA: نمونه جدیدی از Handler را ایجاد می‌کند.
    /// </summary>
    public UpdateExchangeCommandHandler(
        IExchangeRepository exchangeRepository,
        ICountryRepository countryRepository)
    {
        ArgumentNullException.ThrowIfNull(exchangeRepository);
        ArgumentNullException.ThrowIfNull(countryRepository);

        _exchangeRepository = exchangeRepository;
        _countryRepository = countryRepository;
    }

    /// <inheritdoc />
    public async Task<Result<ExchangeId>> Handle(
        UpdateExchangeCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ExchangeId.TryParse(
                request.Id,
                out ExchangeId? exchangeId) ||
            exchangeId is null)
        {
            return Result<ExchangeId>.Fail(
                new Error(
                    "Exchange.InvalidId",
                    "The specified exchange identifier is invalid."));
        }

        Exchange? exchange =
            await _exchangeRepository.GetByIdAsync(
                exchangeId,
                cancellationToken);

        if (exchange is null)
        {
            return Result<ExchangeId>.Fail(
                new Error(
                    "Exchange.NotFound",
                    "The specified exchange was not found."));
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

        CountryId? countryId = null;

        if (!string.IsNullOrWhiteSpace(request.CountryId))
        {
            if (!CountryId.TryParse(
                    request.CountryId,
                    out CountryId? parsedCountryId) ||
                parsedCountryId is null)
            {
                return Result<ExchangeId>.Fail(
                    new Error(
                        "Exchange.InvalidCountryId",
                        "The specified country identifier is invalid."));
            }

            countryId = parsedCountryId;

            Country? country =
                await _countryRepository.GetByIdAsync(
                    countryId,
                    cancellationToken);

            if (country is null)
            {
                return Result<ExchangeId>.Fail(
                    new Error(
                        "Exchange.CountryNotFound",
                        "The specified country was not found."));
            }
        }

        Exchange? existingExchange =
            await _exchangeRepository.GetByCodeAsync(
                code,
                cancellationToken);

        if (existingExchange is not null &&
            existingExchange.Id != exchange.Id)
        {
            return Result<ExchangeId>.Fail(
                new Error(
                    "Exchange.DuplicateCode",
                    "An exchange with the specified code already exists."));
        }

        exchange.ChangeCode(code);
        exchange.Rename(request.Name);

        if (countryId is null)
        {
            exchange.RemoveCountry();
        }
        else
        {
            exchange.AssignCountry(countryId);
        }

        _exchangeRepository.Update(exchange);

        return Result<ExchangeId>.Success(exchange.Id);
    }
}
