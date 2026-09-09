// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.Portfolios
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.Portfolios;

/// <summary>
/// EN: Integration tests for portfolio NAV translated into its configured base currency.
/// FA: تست‌های Integration مربوط به NAV پرتفوی پس از ترجمه به ارز پایه تنظیم‌شده.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioTranslatedNavEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes translated NAV endpoint tests.
    /// FA: تست‌های Endpoint مربوط به NAV ترجمه‌شده را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioTranslatedNavEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies source NAV rows are translated and aggregated with identity and inverse FX conversion.
    /// FA: بررسی می‌کند ردیف‌های NAV مبدا با تبدیل همانی و نرخ معکوس FX ترجمه و سپس تجمیع شوند.
    /// </summary>
    [Fact]
    public async Task TranslatedNav_Should_Translate_And_Aggregate_Multiple_Currencies()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string usdId = await CreateCurrencyAsync("USD", $"US Dollar TNAV {Guid.NewGuid():N}");
        string eurId = await CreateCurrencyAsync("EUR", $"Euro TNAV {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"TNAV Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"TNAV {Guid.NewGuid():N}");

        await SetBaseCurrencyAsync(portfolioId, eurId);
        await CreateCashAsync(new CashRequest(portfolioId, eurId, 1, 500m, DateTimeOffset.UtcNow, null, null, null));
        await CreateCashAsync(new CashRequest(portfolioId, usdId, 1, 1000m, DateTimeOffset.UtcNow.AddSeconds(1), null, null, null));

        await CreateFxRateAsync(
            eurId,
            usdId,
            new DateOnly(2026, 9, 9),
            2m);

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/portfolios/{portfolioId}/translated-nav");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        JsonElement[] currencies = root.GetProperty("currencies").EnumerateArray().ToArray();

        Assert.Equal(portfolioId, root.GetProperty("portfolioId").GetString());
        Assert.Equal(eurId, root.GetProperty("baseCurrencyId").GetString());
        Assert.True(root.GetProperty("isPricedNavFullyTranslated").GetBoolean());
        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal(1000m, root.GetProperty("pricedNetAssetValueBase").GetDecimal());
        Assert.Equal(1000m, root.GetProperty("netAssetValueBase").GetDecimal());

        JsonElement eur =
            currencies.Single(item => item.GetProperty("currencyId").GetString() == eurId);
        JsonElement usd =
            currencies.Single(item => item.GetProperty("currencyId").GetString() == usdId);

        Assert.Equal(1m, eur.GetProperty("sourceToBaseRate").GetDecimal());
        Assert.Equal(500m, eur.GetProperty("netAssetValueBase").GetDecimal());

        Assert.Equal(0.5m, usd.GetProperty("sourceToBaseRate").GetDecimal());
        Assert.Equal(500m, usd.GetProperty("netAssetValueBase").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies missing FX keeps source NAV visible but prevents aggregate base-currency NAV.
    /// FA: بررسی می‌کند نبود FX، NAV مبدا را حفظ کند ولی از ارائه NAV تجمیعی ارز پایه جلوگیری کند.
    /// </summary>
    [Fact]
    public async Task TranslatedNav_When_Fx_Is_Missing_Should_Return_Incomplete_Translation()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string usdId = await CreateCurrencyAsync("USD", $"US Dollar Missing FX {Guid.NewGuid():N}");
        string eurId = await CreateCurrencyAsync("EUR", $"Euro Missing FX {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"TNAV Missing Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"TNAV Missing {Guid.NewGuid():N}");

        await SetBaseCurrencyAsync(portfolioId, eurId);
        await CreateCashAsync(new CashRequest(portfolioId, eurId, 1, 500m, DateTimeOffset.UtcNow, null, null, null));
        await CreateCashAsync(new CashRequest(portfolioId, usdId, 1, 1000m, DateTimeOffset.UtcNow.AddSeconds(1), null, null, null));

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/portfolios/{portfolioId}/translated-nav");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        JsonElement[] currencies = root.GetProperty("currencies").EnumerateArray().ToArray();

        Assert.False(root.GetProperty("isPricedNavFullyTranslated").GetBoolean());
        Assert.False(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("pricedNetAssetValueBase").ValueKind);
        Assert.Equal(JsonValueKind.Null, root.GetProperty("netAssetValueBase").ValueKind);

        JsonElement usd =
            currencies.Single(item => item.GetProperty("currencyId").GetString() == usdId);

        Assert.Equal(1000m, usd.GetProperty("sourceNetAssetValue").GetDecimal());
        Assert.False(usd.GetProperty("isFxAvailable").GetBoolean());
        Assert.Equal(JsonValueKind.Null, usd.GetProperty("sourceToBaseRate").ValueKind);
        Assert.Equal(JsonValueKind.Null, usd.GetProperty("pricedNetAssetValueBase").ValueKind);
        Assert.Equal(JsonValueKind.Null, usd.GetProperty("netAssetValueBase").ValueKind);
    }

    /// <summary>
    /// EN: Verifies unpriced positions keep translated priced NAV available while complete NAV remains unavailable.
    /// FA: بررسی می‌کند موقعیت بدون قیمت، NAV قیمت‌دار ترجمه‌شده را حفظ کند ولی NAV کامل را ناموجود نگه دارد.
    /// </summary>
    [Fact]
    public async Task TranslatedNav_When_Position_Is_Unpriced_Should_Keep_Priced_Nav()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string usdId = await GetListingQuoteCurrencyAsync(listingId);
        string eurId = await CreateCurrencyAsync("EUR", $"Euro Unpriced TNAV {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"TNAV Unpriced Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"TNAV Unpriced {Guid.NewGuid():N}");

        await SetBaseCurrencyAsync(portfolioId, eurId);
        await CreateCashAsync(new CashRequest(portfolioId, usdId, 1, 1000m, DateTimeOffset.UtcNow.AddMinutes(-2), null, null, null));

        await CreateTransactionAsync(
            new TradeRequest(
                portfolioId,
                listingId,
                1,
                4m,
                50m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                DateTimeOffset.UtcNow.AddMinutes(-1)));

        await CreateFxRateAsync(
            eurId,
            usdId,
            new DateOnly(2026, 9, 9),
            2m);

        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/portfolios/{portfolioId}/translated-nav");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        JsonElement currency = root.GetProperty("currencies").EnumerateArray().Single();

        Assert.True(root.GetProperty("isPricedNavFullyTranslated").GetBoolean());
        Assert.False(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal(400m, root.GetProperty("pricedNetAssetValueBase").GetDecimal());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("netAssetValueBase").ValueKind);

        Assert.Equal(800m, currency.GetProperty("sourcePricedNetAssetValue").GetDecimal());
        Assert.False(currency.GetProperty("sourceIsComplete").GetBoolean());
        Assert.Equal(JsonValueKind.Null, currency.GetProperty("sourceNetAssetValue").ValueKind);
        Assert.True(currency.GetProperty("isFxAvailable").GetBoolean());
        Assert.Equal(0.5m, currency.GetProperty("sourceToBaseRate").GetDecimal());
        Assert.Equal(400m, currency.GetProperty("pricedNetAssetValueBase").GetDecimal());
        Assert.Equal(JsonValueKind.Null, currency.GetProperty("netAssetValueBase").ValueKind);
        Assert.Equal(1, currency.GetProperty("unpricedPositionCount").GetInt32());
    }

    private async Task<string> CreateInvestorAsync(string fullName)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/investors",
                new InvestorRequest(fullName));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task<string> CreatePortfolioAsync(string investorId, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolios",
                new PortfolioRequest(investorId, name));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task<string> CreateCurrencyAsync(string code, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/currencies",
                new CurrencyRequest(code, name, 2));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task SetBaseCurrencyAsync(string portfolioId, string currencyId)
    {
        using HttpResponseMessage response =
            await _client.PatchAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/base-currency",
                new SetBaseCurrencyRequest(currencyId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task CreateFxRateAsync(
        string baseCurrencyId,
        string quoteCurrencyId,
        DateOnly rateDate,
        decimal rate)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/fx-rates",
                new FxRateRequest(
                    baseCurrencyId,
                    quoteCurrencyId,
                    rateDate,
                    rate));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private async Task CreateCashAsync(CashRequest request)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolio-cash-transactions",
                request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private async Task CreateTransactionAsync(TradeRequest request)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolio-transactions",
                request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private async Task<string> GetListingQuoteCurrencyAsync(string listingId)
    {
        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/listings/{listingId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? currencyId =
            document.RootElement.GetProperty("quoteCurrencyId").GetString();

        Assert.False(string.IsNullOrWhiteSpace(currencyId));
        return currencyId!;
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        => await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

    private sealed record InvestorRequest(string FullName);

    private sealed record PortfolioRequest(
        string InvestorId,
        string Name);

    private sealed record CurrencyRequest(
        string Code,
        string Name,
        int DecimalPlaces);

    private sealed record SetBaseCurrencyRequest(string CurrencyId);

    private sealed record FxRateRequest(
        string BaseCurrencyId,
        string QuoteCurrencyId,
        DateOnly RateDate,
        decimal Rate);

    private sealed record CashRequest(
        string PortfolioId,
        string CurrencyId,
        int Type,
        decimal Amount,
        DateTimeOffset OccurredOn,
        string? ReferenceType,
        string? ReferenceId,
        string? Description);

    private sealed record TradeRequest(
        string PortfolioId,
        string ListingId,
        int Type,
        decimal Quantity,
        decimal Price,
        decimal Commission,
        decimal Tax,
        decimal ExchangeFee,
        decimal BrokerFee,
        decimal ClearingFee,
        decimal OtherFees,
        DateTimeOffset ExecutedOn);
}
