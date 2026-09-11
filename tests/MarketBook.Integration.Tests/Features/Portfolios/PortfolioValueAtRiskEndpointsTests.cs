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
/// EN: Integration tests for historical VaR and CVaR.
/// FA: تست‌های Integration مربوط به VaR و CVaR تاریخی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioValueAtRiskEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes VaR/CVaR endpoint tests.
    /// FA: تست‌های Endpoint مربوط به VaR/CVaR را مقداردهی می‌کند.
    /// </summary>
    public PortfolioValueAtRiskEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies 95% historical VaR/CVaR on two periodic returns of +10% and -10%.
    /// FA: VaR/CVaR تاریخی 95 درصد را روی دو بازده +10 و -10 درصد بررسی می‌کند.
    /// </summary>
    [Fact]
    public async Task ValueAtRisk_Should_Calculate_Historical_Var_And_Cvar()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("SEK", $"VaR SEK {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"VaR Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"VaR {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, from.AddDays(1), "Dividend"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 5, 110m, to, "Fee"));

        using HttpResponseMessage response =
            await GetValueAtRiskAsync(
                portfolioId,
                from,
                to,
                "Daily",
                0.95m);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.True(root.GetProperty("isCalculable").GetBoolean());
        Assert.Equal(2, root.GetProperty("observationCount").GetInt32());
        Assert.Equal(0.95m, root.GetProperty("confidenceLevel").GetDecimal());
        Assert.Equal(0.05m, root.GetProperty("tailProbability").GetDecimal());
        Assert.Equal(-0.10m, root.GetProperty("historicalQuantileReturn").GetDecimal());
        Assert.Equal(0.10m, root.GetProperty("valueAtRiskReturn").GetDecimal());
        Assert.Equal(0.10m, root.GetProperty("conditionalValueAtRiskReturn").GetDecimal());
        Assert.Equal(1, root.GetProperty("tailObservationCount").GetInt32());
    }

    /// <summary>
    /// EN: Verifies positive-only historical returns do not create a negative VaR loss.
    /// FA: بررسی می‌کند بازده‌های تاریخی تماماً مثبت، زیان VaR منفی تولید نکنند.
    /// </summary>
    [Fact]
    public async Task ValueAtRisk_Should_Floor_Positive_Quantile_Loss_At_Zero()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("NOK", $"VaR NOK {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"VaR Positive Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"VaR Positive {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, from.AddDays(1), "Dividend1"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 55m, to, "Dividend2"));

        using HttpResponseMessage response =
            await GetValueAtRiskAsync(
                portfolioId,
                from,
                to,
                "Daily",
                0.95m);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isCalculable").GetBoolean());
        Assert.True(root.GetProperty("historicalQuantileReturn").GetDecimal() > 0m);
        Assert.Equal(0m, root.GetProperty("valueAtRiskReturn").GetDecimal());
        Assert.Equal(0m, root.GetProperty("conditionalValueAtRiskReturn").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies an invalid confidence level is rejected.
    /// FA: بررسی می‌کند سطح اطمینان نامعتبر رد شود.
    /// </summary>
    [Fact]
    public async Task ValueAtRisk_Should_Reject_Invalid_Confidence_Level()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("DKK", $"VaR DKK {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"VaR Invalid Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"VaR Invalid {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);

        using HttpResponseMessage response =
            await GetValueAtRiskAsync(
                portfolioId,
                from,
                to,
                "Daily",
                1.00m);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<HttpResponseMessage> GetValueAtRiskAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval,
        decimal confidenceLevel)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/risk/var" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
            $"&interval={Uri.EscapeDataString(interval)}" +
            $"&confidenceLevel={confidenceLevel.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

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
