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
/// EN: Integration tests for monetary drawdown episode analytics.
/// FA: تست‌های Integration تحلیل دوره‌های افت مبلغی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioMonetaryDrawdownEpisodesEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes monetary drawdown episode endpoint tests.
    /// FA: تست‌های Endpoint دوره افت مبلغی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioMonetaryDrawdownEpisodesEndpointsTests(
        IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies a recovered episode reports trough loss and the same amount as recovered capital.
    /// FA: بررسی می‌کند دوره بازیابی‌شده زیان کف و همان مبلغ را به‌عنوان سرمایه بازیابی‌شده گزارش کند.
    /// </summary>
    [Fact]
    public async Task MonetaryEpisodes_Should_Report_Recovered_Trough_Amount()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "HUF",
                $"Monetary Episodes HUF {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Monetary Episodes Recovered Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Monetary Episodes Recovered {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

        DateTimeOffset trough =
            from.AddDays(1);

        DateTimeOffset recovery =
            from.AddDays(2);

        await SetBaseCurrencyAsync(
            portfolioId,
            currencyId);

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                1,
                1000m,
                from.AddDays(-1),
                null));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                5,
                100m,
                trough,
                "Fee"));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                7,
                100m,
                recovery,
                "Dividend"));

        using HttpResponseMessage response =
            await GetMonetaryEpisodesAsync(
                portfolioId,
                from,
                recovery,
                "Daily");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        JsonElement root =
            document.RootElement;

        Assert.True(
            root.GetProperty("isComplete").GetBoolean());

        Assert.False(
            root.GetProperty("hasActiveDrawdown").GetBoolean());

        Assert.Equal(
            1,
            root.GetProperty("episodeCount").GetInt32());

        JsonElement episode =
            root.GetProperty("episodes")[0];

        Assert.True(
            episode.GetProperty("isRecovered").GetBoolean());

        Assert.True(
            episode.GetProperty("isComplete").GetBoolean());

        Assert.Equal(
            -0.10m,
            episode.GetProperty("maximumDrawdown").GetDecimal());

        Assert.Equal(
            900m,
            episode.GetProperty("troughNetAssetValueBase").GetDecimal());

        Assert.Equal(
            1000m,
            episode.GetProperty("troughEquivalentPeakNetAssetValueBase").GetDecimal());

        Assert.Equal(
            100m,
            episode.GetProperty("troughDrawdownAmountBase").GetDecimal());

        Assert.Equal(
            1000m,
            episode.GetProperty("recoveryNetAssetValueBase").GetDecimal());

        Assert.Equal(
            100m,
            episode.GetProperty("recoveredDrawdownAmountBase").GetDecimal());

        Assert.Equal(
            JsonValueKind.Null,
            episode.GetProperty("currentDrawdownAmountBase").ValueKind);
    }

    /// <summary>
    /// EN: Verifies an ongoing episode reports current drawdown amount at exact To.
    /// FA: بررسی می‌کند دوره جاری مبلغ افت فعال را در To دقیق گزارش کند.
    /// </summary>
    [Fact]
    public async Task MonetaryEpisodes_Should_Report_Active_Drawdown_Amount_At_To()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "RON",
                $"Monetary Episodes RON {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Monetary Episodes Active Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Monetary Episodes Active {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

        DateTimeOffset trough =
            from.AddDays(1);

        DateTimeOffset to =
            from.AddDays(2);

        await SetBaseCurrencyAsync(
            portfolioId,
            currencyId);

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                1,
                1000m,
                from.AddDays(-1),
                null));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                5,
                100m,
                trough,
                "Fee"));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                7,
                50m,
                to,
                "Dividend"));

        using HttpResponseMessage response =
            await GetMonetaryEpisodesAsync(
                portfolioId,
                from,
                to,
                "Daily");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        JsonElement root =
            document.RootElement;

        Assert.True(
            root.GetProperty("isComplete").GetBoolean());

        Assert.True(
            root.GetProperty("hasActiveDrawdown").GetBoolean());

        Assert.Equal(
            1,
            root.GetProperty("episodeCount").GetInt32());

        JsonElement active =
            root.GetProperty("activeDrawdownEpisode");

        Assert.False(
            active.GetProperty("isRecovered").GetBoolean());

        Assert.True(
            active.GetProperty("isComplete").GetBoolean());

        Assert.Equal(
            100m,
            active.GetProperty("troughDrawdownAmountBase").GetDecimal());

        Assert.Equal(
            50m,
            active.GetProperty("currentDrawdownAmountBase").GetDecimal());

        Assert.Equal(
            JsonValueKind.Null,
            active.GetProperty("recoveredDrawdownAmountBase").ValueKind);
    }

    /// <summary>
    /// EN: Verifies invalid interval is rejected by the composed episode contract.
    /// FA: بررسی می‌کند فاصله نمونه‌برداری نامعتبر توسط قرارداد ترکیبی episode رد شود.
    /// </summary>
    [Fact]
    public async Task MonetaryEpisodes_Should_Reject_Invalid_Interval()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "BGN",
                $"Monetary Episodes BGN {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"Monetary Episodes Invalid Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"Monetary Episodes Invalid {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

        await SetBaseCurrencyAsync(
            portfolioId,
            currencyId);

        using HttpResponseMessage response =
            await GetMonetaryEpisodesAsync(
                portfolioId,
                from,
                from.AddDays(2),
                "Monthly");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private async Task<HttpResponseMessage> GetMonetaryEpisodesAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/drawdown/episodes/amount" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
            $"&interval={Uri.EscapeDataString(interval)}");

    private async Task<string> CreateCurrencyAsync(
        string code,
        string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/currencies",
                new CurrencyRequest(
                    code,
                    name,
                    2));

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        return document.RootElement
            .GetProperty("id")
            .GetString()!;
    }

    private async Task<string> CreateInvestorAsync(
        string fullName)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/investors",
                new InvestorRequest(fullName));

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        return document.RootElement
            .GetProperty("id")
            .GetString()!;
    }

    private async Task<string> CreatePortfolioAsync(
        string investorId,
        string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolios",
                new PortfolioRequest(
                    investorId,
                    name));

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        using JsonDocument document =
            await ReadJsonAsync(response);

        return document.RootElement
            .GetProperty("id")
            .GetString()!;
    }

    private async Task SetBaseCurrencyAsync(
        string portfolioId,
        string currencyId)
    {
        using HttpResponseMessage response =
            await _client.PatchAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/base-currency",
                new SetBaseCurrencyRequest(currencyId));

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    private async Task CreateCashAsync(
        CashRequest request)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolio-cash-transactions",
                request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    private static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response)
        => await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync());

    private sealed record CurrencyRequest(
        string Code,
        string Name,
        int DecimalPlaces);

    private sealed record InvestorRequest(
        string FullName);

    private sealed record PortfolioRequest(
        string InvestorId,
        string Name);

    private sealed record SetBaseCurrencyRequest(
        string CurrencyId);

    private sealed record CashRequest(
        string PortfolioId,
        string CurrencyId,
        int Type,
        decimal Amount,
        DateTimeOffset OccurredOn,
        string? Description);
}
