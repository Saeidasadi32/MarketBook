// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Queries.GetAllCurrencies
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MediatR;

namespace MarketBook.Application.Features.Currencies.Queries.GetAllCurrencies;

/// <summary>
/// EN: Handles retrieving a paged collection of currencies.
/// FA: دریافت مجموعه صفحه‌بندی‌شده ارزها را مدیریت می‌کند.
/// </summary>
public sealed class GetAllCurrenciesHandler
    : IRequestHandler<GetAllCurrenciesQuery, Result<GetAllCurrenciesResponse>>
{
    private readonly ICurrencyRepository _currencyRepository;

    /// <summary>
    /// EN: Initializes a new instance of the Get All Currencies query handler.
    /// FA: یک نمونه جدید از Handler پرس‌وجوی دریافت همه ارزها را ایجاد می‌کند.
    /// </summary>
    /// <param name="currencyRepository">
    /// EN: Currency repository.
    /// FA: Repository ارز.
    /// </param>
    public GetAllCurrenciesHandler(ICurrencyRepository currencyRepository)
    {
        ArgumentNullException.ThrowIfNull(currencyRepository);
        _currencyRepository = currencyRepository;
    }

    /// <summary>
    /// EN: Handles the request to retrieve a paged collection of currencies.
    /// FA: درخواست دریافت مجموعه صفحه‌بندی‌شده ارزها را پردازش می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Get All Currencies query.
    /// FA: پرس‌وجوی دریافت همه ارزها.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: A paged currency response when successful; otherwise a domain error.
    /// FA: پاسخ صفحه‌بندی‌شده ارزها در صورت موفقیت؛ در غیر این صورت خطای دامنه.
    /// </returns>
    public async Task<Result<GetAllCurrenciesResponse>> Handle(
        GetAllCurrenciesQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<Currency> pagedResult =
            await _currencyRepository.GetPagedAsync(
                pageRequest,
                cancellationToken);

        CurrencyListItemResponse[] items =
            pagedResult.Items
                .Select(currency =>
                    new CurrencyListItemResponse(
                        currency.Id.Value.ToString(),
                        currency.Code.Value,
                        currency.Name,
                        currency.DecimalPlaces,
                        currency.CreatedOn,
                        currency.IsActive))
                .ToArray();

        GetAllCurrenciesResponse response = new(
            items,
            pagedResult.Page,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages);

        return Result<GetAllCurrenciesResponse>.Success(response);
    }
}
