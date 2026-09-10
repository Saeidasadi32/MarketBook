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
/// EN: Integration tests for historical portfolio NAV translated into base currency.
/// FA: تست‌های Integration مربوط به NAV تاریخی پرتفوی پس از ترجمه به ارز پایه.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioTranslatedNavAsOfEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes historical translated NAV endpoint tests.
    /// FA: تست‌های Endpoint مربوط به NAV تاریخی ترجمه‌شده را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioTranslatedNavAsOfEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies historical translation selects the latest eligible FX rate and ignores a newer future rate.
    /// FA: بررسی می‌کند ترجمه تاریخی آخرین نرخ FX مجاز را انتخاب کرده و نرخ جدیدتر آینده را نادیده بگیرد.
    /// </summary>
    [Fact]
    public async Task TranslatedNavAsOf_Should_Ignore_Future_Fx_And_Use_Latest_Eligible_Rate()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string usdId = await GetListingQuoteCurrencyAsync(listingId);
        string eurId = await CreateCurrencyAsync("EUR", $"Euro AsOf FX {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"AsOf FX Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"AsOf FX {Guid.NewGuid():N}");

        DateTimeOffset cutoff = new(2026, 9, 9, 18, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, eurId);
        await CreateCashAsync(new CashRequest(
            portfolioId, eurId, 1, 500m, cutoff.AddDays(-2), null, null, null));
        await CreateCashAsync(new CashRequest(
            portfolioId, usdId, 1, 1000m, cutoff.AddDays(-1), null, null, null));

        await CreateFxRateAsync(
            usdId,
            eurId,
            new DateOnly(2026, 9, 8),
            0.4m);

        await CreateFxRateAsync(
            eurId,
            usdId,
            new DateOnly(2026, 9, 10),
            2m);

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolios/{portfolioId}/translated-nav/as-of?asOf={Uri.EscapeDataString(cutoff.ToString("O"))}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        JsonElement[] currencies = root.GetProperty("currencies").EnumerateArray().ToArray();

        Assert.Equal(portfolioId, root.GetProperty("portfolioId").GetString());
        Assert.Equal(eurId, root.GetProperty("baseCurrencyId").GetString());
        Assert.True(root.GetProperty("isPricedNavFullyTranslated").GetBoolean());
        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal(900m, root.GetProperty("pricedNetAssetValueBase").GetDecimal());
        Assert.Equal(900m, root.GetProperty("netAssetValueBase").GetDecimal());

        JsonElement usd =
            currencies.Single(item => item.GetProperty("currencyId").GetString() == usdId);

        string? fxRateDateText =
    usd.GetProperty("fxRateDate").GetString();

        Assert.False(string.IsNullOrWhiteSpace(fxRateDateText));

        DateOnly fxRateDate =
            DateOnly.Parse(fxRateDateText);

        Assert.Equal(new DateOnly(2026, 9, 8), fxRateDate);
        Assert.Equal(0.4m, usd.GetProperty("sourceToBaseRate").GetDecimal());
        Assert.Equal(400m, usd.GetProperty("netAssetValueBase").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies a future FX rate is not considered available for an earlier historical NAV.
    /// FA: بررسی می‌کند نرخ FX آینده برای NAV تاریخی قبل از آن قابل استفاده تلقی نشود.
    /// </summary>
    [Fact]
    public async Task TranslatedNavAsOf_When_Only_Future_Fx_Exists_Should_Return_Incomplete_Translation()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string usdId = await GetListingQuoteCurrencyAsync(listingId);
        string eurId = await CreateCurrencyAsync("EUR", $"Euro Future FX {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Future FX Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Future FX {Guid.NewGuid():N}");

        DateTimeOffset cutoff = new(2026, 9, 9, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, eurId);
        await CreateCashAsync(new CashRequest(
            portfolioId, usdId, 1, 1000m, cutoff.AddDays(-1), null, null, null));

        await CreateFxRateAsync(
            eurId,
            usdId,
            new DateOnly(2026, 9, 10),
            2m);

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolios/{portfolioId}/translated-nav/as-of?asOf={Uri.EscapeDataString(cutoff.ToString("O"))}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        JsonElement usd = root.GetProperty("currencies").EnumerateArray().Single();

        Assert.False(root.GetProperty("isPricedNavFullyTranslated").GetBoolean());
        Assert.False(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("pricedNetAssetValueBase").ValueKind);
        Assert.Equal(JsonValueKind.Null, root.GetProperty("netAssetValueBase").ValueKind);

        Assert.Equal(1000m, usd.GetProperty("sourceNetAssetValue").GetDecimal());
        Assert.False(usd.GetProperty("isFxAvailable").GetBoolean());
        Assert.Equal(JsonValueKind.Null, usd.GetProperty("fxRateDate").ValueKind);
        Assert.Equal(JsonValueKind.Null, usd.GetProperty("sourceToBaseRate").ValueKind);
        Assert.Equal(JsonValueKind.Null, usd.GetProperty("netAssetValueBase").ValueKind);
    }

    /// <summary>
    /// EN: Verifies historical FX can translate priced NAV even when no historical market price exists for an open position.
    /// FA: بررسی می‌کند حتی در نبود قیمت تاریخی یک موقعیت باز، FX تاریخی بتواند NAV قیمت‌دار را ترجمه کند.
    /// </summary>
    [Fact]
    public async Task TranslatedNavAsOf_When_Position_Is_Historically_Unpriced_Should_Keep_Translated_Priced_Nav()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string usdId = await GetListingQuoteCurrencyAsync(listingId);
        string eurId = await CreateCurrencyAsync("EUR", $"Euro Historical Price {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Historical Price Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Historical Price {Guid.NewGuid():N}");

        DateTimeOffset cutoff = new(2026, 9, 9, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(portfolioId, eurId);
        await CreateCashAsync(new CashRequest(
            portfolioId, usdId, 1, 1000m, cutoff.AddDays(-2), null, null, null));

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
                cutoff.AddDays(-1)));

        await CreateMarketPriceAsync(
            listingId,
            new DateOnly(2026, 9, 10),
            130m);

        await CreateFxRateAsync(
            eurId,
            usdId,
            new DateOnly(2026, 9, 8),
            2m);

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolios/{portfolioId}/translated-nav/as-of?asOf={Uri.EscapeDataString(cutoff.ToString("O"))}");

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
        Assert.Equal(0.5m, currency.GetProperty("sourceToBaseRate").GetDecimal());
        Assert.Equal(400m, currency.GetProperty("pricedNetAssetValueBase").GetDecimal());
        Assert.Equal(JsonValueKind.Null, currency.GetProperty("netAssetValueBase").ValueKind);
        Assert.Equal(1, currency.GetProperty("unpricedPositionCount").GetInt32());
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

    private async Task SetBaseCurrencyAsync(string portfolioId, string currencyId)
    {
        using HttpResponseMessage response =
            await _client.PatchAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/base-currency",
                new SetBaseCurrencyRequest(currencyId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<string> GetListingQuoteCurrencyAsync(string listingId)
    {
        using HttpResponseMessage response =
            await _client.GetAsync($"/api/v1/listings/{listingId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? currencyId = document.RootElement.GetProperty("quoteCurrencyId").GetString();

        Assert.False(string.IsNullOrWhiteSpace(currencyId));
        return currencyId!;
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

    private async Task CreateMarketPriceAsync(
        string listingId,
        DateOnly tradingDate,
        decimal lastPrice)
    {
        MarketPriceRequest request = new(
            listingId,
            tradingDate,
            lastPrice,
            lastPrice + 5m,
            lastPrice - 5m,
            lastPrice,
            lastPrice,
            lastPrice - 1m,
            lastPrice,
            lastPrice - 20m,
            lastPrice + 20m);

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/market-prices",
                request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        => await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

    private sealed record CurrencyRequest(string Code, string Name, int DecimalPlaces);
    private sealed record InvestorRequest(string FullName);
    private sealed record PortfolioRequest(string InvestorId, string Name);
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
    private sealed record MarketPriceRequest(
        string ListingId,
        DateOnly TradingDate,
        decimal OpenPrice,
        decimal HighPrice,
        decimal LowPrice,
        decimal LastPrice,
        decimal ClosePrice,
        decimal PreviousClosePrice,
        decimal ReferencePrice,
        decimal LowerLimit,
        decimal UpperLimit);
}
