// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Tests
// Namespace : MarketBook.Integration.Tests.Features.PortfolioTransactions
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MarketBook.Integration.Tests.Infrastructure;

namespace MarketBook.Integration.Tests.Features.PortfolioTransactions;

/// <summary>
/// EN: Integration tests for portfolio base currency and FX-translated P/L.
/// FA: ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ Integration Ù…Ø±Ø¨ÙˆØ· Ø¨Ù‡ Ø§Ø±Ø² Ù¾Ø§ÛŒÙ‡ Ù¾Ø±ØªÙÙˆÛŒ Ùˆ Ø³ÙˆØ¯/Ø²ÛŒØ§Ù† ØªØ±Ø¬Ù…Ù‡â€ŒØ´Ø¯Ù‡ Ø¨Ø§ FX.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioFxTranslationEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes FX translation endpoint tests.
    /// FA: ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ Endpoint ØªØ±Ø¬Ù…Ù‡ FX Ø±Ø§ Ù…Ù‚Ø¯Ø§Ø±Ø¯Ù‡ÛŒ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.
    /// </summary>
    public PortfolioFxTranslationEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies portfolio base currency can be configured and is returned by portfolio lookup.
    /// FA: Ø¨Ø±Ø±Ø³ÛŒ Ù…ÛŒâ€ŒÚ©Ù†Ø¯ Ø§Ø±Ø² Ù¾Ø§ÛŒÙ‡ Ù¾Ø±ØªÙÙˆÛŒ Ù‚Ø§Ø¨Ù„ ØªÙ†Ø¸ÛŒÙ… Ø§Ø³Øª Ùˆ Ø¯Ø± Ø¯Ø±ÛŒØ§ÙØª Ù¾Ø±ØªÙÙˆÛŒ Ø¨Ø±Ú¯Ø±Ø¯Ø§Ù†Ø¯Ù‡ Ù…ÛŒâ€ŒØ´ÙˆØ¯.
    /// </summary>
    [Fact]
    public async Task Portfolio_Should_Set_And_Return_BaseCurrency()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        await _fixture.CreatePortfolioTransactionListingAsync();
        string euroId = await CreateCurrencyAsync("EUR", "Euro Test");
        string investorId = await CreateInvestorAsync($"FX Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"FX Portfolio {Guid.NewGuid():N}");

        using HttpResponseMessage patchResponse =
            await _client.PatchAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/base-currency",
                new SetBaseCurrencyRequest(euroId));

        Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

        using HttpResponseMessage getResponse =
            await _client.GetAsync($"/api/v1/portfolios/{portfolioId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        using JsonDocument document = await ReadJsonAsync(getResponse);

        Assert.Equal(
            euroId,
            document.RootElement.GetProperty("baseCurrencyId").GetString());
    }

    /// <summary>
    /// EN: Verifies duplicate FX quotes for the same pair/date are rejected.
    /// FA: Ø¨Ø±Ø±Ø³ÛŒ Ù…ÛŒâ€ŒÚ©Ù†Ø¯ Ù†Ø±Ø® ØªÚ©Ø±Ø§Ø±ÛŒ Ø¨Ø±Ø§ÛŒ ÛŒÚ© Ø¬ÙØªâ€ŒØ§Ø±Ø² Ùˆ ØªØ§Ø±ÛŒØ® Ø±Ø¯ Ø´ÙˆØ¯.
    /// </summary>
    [Fact]
    public async Task FxRate_Should_Reject_Duplicate_Pair_And_Date()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        await _fixture.CreatePortfolioTransactionListingAsync();
        string usdId = await GetCurrencyIdByCodeAsync("USD");
        string gbpId  = await CreateCurrencyAsync("GBP", "British Pound FX");
        DateOnly rateDate = new(2026, 9, 8);

        FxRateRequest request = new(
            gbpId ,
            usdId,
            rateDate,
            2m);

        using HttpResponseMessage first =
            await _client.PostAsJsonAsync("/api/v1/fx-rates", request);

        using HttpResponseMessage second =
            await _client.PostAsJsonAsync("/api/v1/fx-rates", request);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    /// <summary>
    /// EN: Verifies translated total P/L uses the newest available pair rate and handles inverse direction.
    /// FA: Ø¨Ø±Ø±Ø³ÛŒ Ù…ÛŒâ€ŒÚ©Ù†Ø¯ Ø³ÙˆØ¯/Ø²ÛŒØ§Ù† ØªØ±Ø¬Ù…Ù‡â€ŒØ´Ø¯Ù‡ Ø§Ø² Ø¬Ø¯ÛŒØ¯ØªØ±ÛŒÙ† Ù†Ø±Ø® Ø¬ÙØªâ€ŒØ§Ø±Ø² Ø§Ø³ØªÙØ§Ø¯Ù‡ Ú©Ø±Ø¯Ù‡ Ùˆ Ø¬Ù‡Øª Ù…Ø¹Ú©ÙˆØ³ Ø±Ø§ Ù†ÛŒØ² Ù…Ø¯ÛŒØ±ÛŒØª Ú©Ù†Ø¯.
    /// </summary>
    [Fact]
    public async Task TranslatedPnl_Should_Use_Latest_Available_Fx_Rate()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string listingId = await _fixture.CreatePortfolioTransactionListingAsync();
        string usdId = await GetCurrencyIdByCodeAsync("USD");
        string chfId  = await CreateCurrencyAsync("CHF", "Swiss Franc Translate");
        string investorId = await CreateInvestorAsync($"Translate Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Translate {Guid.NewGuid():N}");

        await SetBaseCurrencyAsync(portfolioId, chfId);

        await CreateTransactionAsync(
            new PortfolioTransactionRequest(
                portfolioId,
                listingId,
                1,
                10m,
                100m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                DateTimeOffset.UtcNow.AddMinutes(-3)));

        await CreateTransactionAsync(
            new PortfolioTransactionRequest(
                portfolioId,
                listingId,
                1,
                10m,
                200m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                DateTimeOffset.UtcNow.AddMinutes(-2)));

        await CreateTransactionAsync(
            new PortfolioTransactionRequest(
                portfolioId,
                listingId,
                2,
                5m,
                250m,
                0m,
                0m,
                0m,
                0m,
                0m,
                0m,
                DateTimeOffset.UtcNow.AddMinutes(-1)));

        await CreateMarketPriceAsync(
            listingId,
            new DateOnly(2026, 9, 8),
            180m);

        await CreateFxRateAsync(
            usdId,
            chfId ,
            new DateOnly(2026, 9, 7),
            0.4m);

        await CreateFxRateAsync(
            chfId ,
            usdId,
            new DateOnly(2026, 9, 8),
            2m);

        using HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/v1/portfolio-transactions/translated-pnl?portfolioId={portfolioId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;
        JsonElement currency = root.GetProperty("currencies").EnumerateArray().Single();

        Assert.Equal(chfId , root.GetProperty("baseCurrencyId").GetString());
        Assert.True(root.GetProperty("isRealizedFullyTranslated").GetBoolean());
        Assert.True(root.GetProperty("isFullyPricedAndTranslated").GetBoolean());

        Assert.Equal(250m, root.GetProperty("realizedProfitLossBase").GetDecimal());
        Assert.Equal(225m, root.GetProperty("unrealizedProfitLossBase").GetDecimal());
        Assert.Equal(475m, root.GetProperty("totalProfitLossBase").GetDecimal());

        Assert.Equal(0.5m, currency.GetProperty("sourceToBaseRate").GetDecimal());
        Assert.Equal(475m, currency.GetProperty("totalProfitLossBase").GetDecimal());
    }

    private async Task<string> CreateCurrencyAsync(string code, string name)
    {
        string normalizedCode =
            code.Length >= 3
                ? code[..3]
                : code.PadRight(3, 'X');

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/currencies",
                new CurrencyRequest(normalizedCode, name, 2));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        string? id = document.RootElement.GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task<string> GetCurrencyIdByCodeAsync(string code)
    {
        using HttpResponseMessage response =
            await _client.GetAsync("/api/v1/currencies?page=1&pageSize=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);

        JsonElement item =
            document.RootElement.GetProperty("items")
                .EnumerateArray()
                .Single(element =>
                    string.Equals(
                        element.GetProperty("code").GetString(),
                        code,
                        StringComparison.OrdinalIgnoreCase));

        return item.GetProperty("id").GetString()!;
    }

    private async Task<string> CreateInvestorAsync(string fullName)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/investors",
                new InvestorRequest(fullName));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task<string> CreatePortfolioAsync(
        string investorId,
        string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolios",
                new PortfolioRequest(investorId, name));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task SetBaseCurrencyAsync(
        string portfolioId,
        string currencyId)
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

    private async Task CreateTransactionAsync(
        PortfolioTransactionRequest request)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolio-transactions",
                request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private async Task CreateMarketPriceAsync(
        string listingId,
        DateOnly tradingDate,
        decimal lastPrice)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/market-prices",
                new MarketPriceRequest(
                    listingId,
                    tradingDate,
                    lastPrice,
                    lastPrice,
                    lastPrice,
                    lastPrice,
                    lastPrice,
                    lastPrice,
                    lastPrice,
                    lastPrice - 20m,
                    lastPrice + 20m));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response)
        => await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

    private sealed record CurrencyRequest(
        string Code,
        string Name,
        int DecimalPlaces);

    private sealed record InvestorRequest(string FullName);

    private sealed record PortfolioRequest(
        string InvestorId,
        string Name);

    private sealed record SetBaseCurrencyRequest(string CurrencyId);

    private sealed record FxRateRequest(
        string BaseCurrencyId,
        string QuoteCurrencyId,
        DateOnly RateDate,
        decimal Rate);

    private sealed record PortfolioTransactionRequest(
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

