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
/// EN: Integration tests for drawdown duration and recovery analytics.
/// FA: تست‌های Integration تحلیل مدت افت و بازیابی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioDrawdownEpisodesEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes endpoint tests.
    /// FA: تست‌های Endpoint را مقداردهی می‌کند.
    /// </summary>
    public PortfolioDrawdownEpisodesEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies a fee drawdown followed by dividend recovery produces one recovered episode.
    /// FA: بررسی می‌کند افت ناشی از Fee و سپس بازیابی با Dividend یک دوره بازیابی‌شده ایجاد کند.
    /// </summary>
    [Fact]
    public async Task Episodes_Should_Capture_Peak_Trough_And_Recovery()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("EUR", $"Episodes EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Episodes Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Episodes {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset trough = from.AddDays(1);
        DateTimeOffset recovery = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 200m, trough, "Fee"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 400m, recovery, "Dividend"));

        using HttpResponseMessage response =
            await GetEpisodesAsync(portfolioId, from, recovery, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.False(root.GetProperty("hasActiveDrawdown").GetBoolean());
        Assert.Equal(1, root.GetProperty("episodeCount").GetInt32());

        JsonElement episode = root.GetProperty("episodes")[0];

        Assert.True(episode.GetProperty("isRecovered").GetBoolean());
        Assert.Equal(-0.2m, episode.GetProperty("maximumDrawdown").GetDecimal());
        Assert.Equal(from, DateTimeOffset.Parse(episode.GetProperty("peakTimestamp").GetString()!));
        Assert.Equal(trough, DateTimeOffset.Parse(episode.GetProperty("troughTimestamp").GetString()!));
        Assert.Equal(recovery, DateTimeOffset.Parse(episode.GetProperty("recoveryTimestamp").GetString()!));
        Assert.Equal(1m, episode.GetProperty("peakToTroughDays").GetDecimal());
        Assert.Equal(1m, episode.GetProperty("recoveryDays").GetDecimal());
        Assert.Equal(2m, episode.GetProperty("totalDurationDays").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies an unrecovered loss remains active through exact To.
    /// FA: بررسی می‌کند افت بازیابی‌نشده تا To دقیق به‌صورت فعال باقی بماند.
    /// </summary>
    [Fact]
    public async Task Episodes_Should_Expose_Ongoing_Drawdown_Through_To()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("GBP", $"Episodes GBP {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Episodes Active Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Episodes Active {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset trough = from.AddDays(1);
        DateTimeOffset to = from.AddDays(3);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 200m, trough, "Fee"));

        using HttpResponseMessage response =
            await GetEpisodesAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("hasActiveDrawdown").GetBoolean());

        JsonElement active = root.GetProperty("activeDrawdownEpisode");

        Assert.False(active.GetProperty("isRecovered").GetBoolean());
        Assert.Equal(-0.2m, active.GetProperty("maximumDrawdown").GetDecimal());
        Assert.Equal(3m, active.GetProperty("totalDurationDays").GetDecimal());
        Assert.Equal(JsonValueKind.Null, active.GetProperty("recoveryTimestamp").ValueKind);
        Assert.Equal(JsonValueKind.Null, active.GetProperty("recoveryDays").ValueKind);
    }

    /// <summary>
    /// EN: Verifies external capital flows do not create a drawdown episode.
    /// FA: بررسی می‌کند جریان سرمایه خارجی به‌تنهایی دوره افت ایجاد نکند.
    /// </summary>
    [Fact]
    public async Task Episodes_Should_Ignore_Deposit_And_Withdrawal()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("CHF", $"Episodes CHF {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Episodes Flow Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Episodes Flow {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 2, 200m, from.AddDays(1), "Withdrawal"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 400m, to, "Deposit"));

        using HttpResponseMessage response =
            await GetEpisodesAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.False(root.GetProperty("hasActiveDrawdown").GetBoolean());
        Assert.Equal(0, root.GetProperty("episodeCount").GetInt32());
        Assert.Equal(0, root.GetProperty("episodes").GetArrayLength());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("maximumDrawdownEpisode").ValueKind);
        Assert.Equal(JsonValueKind.Null, root.GetProperty("longestDrawdownEpisode").ValueKind);
    }

    private async Task<HttpResponseMessage> GetEpisodesAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/drawdown/episodes" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
            $"&interval={Uri.EscapeDataString(interval)}");

    private async Task<string> CreateCurrencyAsync(string code, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/currencies",
                new CurrencyRequest(code, name, 2));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
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

    private async Task<string> CreatePortfolioAsync(string investorId, string name)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolios",
                new PortfolioRequest(investorId, name));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        return document.RootElement.GetProperty("id").GetString()!;
    }

    private async Task SetBaseCurrencyAsync(string portfolioId, string currencyId)
    {
        using HttpResponseMessage response =
            await _client.PatchAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/base-currency",
                new SetBaseCurrencyRequest(currencyId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task CreateCashAsync(CashRequest request)
    {
        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolio-cash-transactions",
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
    private sealed record CashRequest(
        string PortfolioId,
        string CurrencyId,
        int Type,
        decimal Amount,
        DateTimeOffset OccurredOn,
        string? Description);
}
