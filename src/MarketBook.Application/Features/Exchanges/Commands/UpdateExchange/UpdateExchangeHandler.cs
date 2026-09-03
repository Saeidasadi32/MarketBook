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
/// EN: Handles requests to update an existing exchange.
/// FA: درخواست‌های به‌روزرسانی یک بورس موجود را پردازش می‌کند.
/// </summary>
public sealed class UpdateExchangeHandler
    : IRequestHandler<
        UpdateExchangeCommand,
        Result<ExchangeId>>
{
    private readonly IExchangeRepository _exchangeRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new instance of the handler.
    /// FA: نمونه جدیدی از Handler را ایجاد می‌کند.
    /// </summary>
    /// <param name="exchangeRepository">
    /// EN: Exchange repository.
    /// FA: Repository مربوط به بورس.
    /// </param>
    /// <param name="countryRepository">
    /// EN: Country repository.
    /// FA: Repository مربوط به کشور.
    /// </param>
    /// <param name="dbContext">
    /// EN: Application database context.
    /// FA: کانتکست پایگاه داده برنامه.
    /// </param>
    public UpdateExchangeHandler(
        IExchangeRepository exchangeRepository,
        ICountryRepository countryRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(exchangeRepository);
        ArgumentNullException.ThrowIfNull(countryRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _exchangeRepository = exchangeRepository;
        _countryRepository = countryRepository;
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Result<ExchangeId>> Handle(
        UpdateExchangeCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ExchangeId? exchangeId = ExchangeId.TryParse(
            request.Id,
            out ExchangeId? parsedExchangeId)
            ? parsedExchangeId
            : null;

        if (exchangeId is null)
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

        if (await _exchangeRepository.ExistsAsync(
                code,
                exchangeId,
                cancellationToken))
        {
            return Result<ExchangeId>.Fail(
                new Error(
                    "Exchange.DuplicateCode",
                    "An exchange with the specified code already exists."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<ExchangeId>.Fail(
                new Error(
                    "Exchange.InvalidName",
                    "Exchange name is required."));
        }

        exchange.ChangeCode(code);
        exchange.Rename(request.Name);

        if (string.IsNullOrWhiteSpace(request.CountryId))
        {
            exchange.RemoveCountry();
        }
        else
        {
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

            exchange.AssignCountry(countryId);
        }

        _exchangeRepository.Update(exchange);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result<ExchangeId>.Success(exchange.Id);
    }
}
