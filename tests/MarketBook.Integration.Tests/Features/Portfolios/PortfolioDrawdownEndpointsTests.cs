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
/// EN: Integration tests for portfolio drawdown analytics.
/// FA: تست‌های Integration مربوط به تحلیل افت سرمایه پرتفوی.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioDrawdownEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes drawdown endpoint tests.
    /// FA: تست‌های Endpoint افت سرمایه را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration-test fixture. FA: Fixture تست یکپارچه.</param>
    public PortfolioDrawdownEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies a dividend-driven NAV rise creates no drawdown.
    /// FA: بررسی می‌کند افزایش NAV ناشی از سود نقدی هیچ افت سرمایه‌ای ایجاد نکند.
    /// </summary>
    [Fact]
    public async Task Drawdown_Should_Remain_Zero_When_Nav_Only_Rises()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("EUR", $"Drawdown EUR {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Drawdown Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Drawdown {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 7, 100m, from.AddDays(1), "Dividend"));

        using HttpResponseMessage response =
            await GetDrawdownAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal(0m, root.GetProperty("currentDrawdown").GetDecimal());
        Assert.Equal(0m, root.GetProperty("maximumDrawdown").GetDecimal());
        Assert.Equal(1.1m, root.GetProperty("peakWealthIndex").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies a withdrawal is neutralized by NAV itself and does not fabricate investment drawdown.
    /// FA: بررسی می‌کند برداشت سرمایه توسط خود NAV خنثی شده و افت سرمایه ساختگی ایجاد نکند.
    /// </summary>
    [Fact]
    public async Task Drawdown_Should_Reflect_Nav_Even_When_External_Withdrawal_Occurs()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("GBP", $"Drawdown GBP {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Drawdown Withdrawal Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Drawdown Withdrawal {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 2, 200m, from.AddDays(1), "Withdrawal"));

        using HttpResponseMessage response =
            await GetDrawdownAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.Equal(0m, root.GetProperty("currentDrawdown").GetDecimal());
        Assert.Equal(0m, root.GetProperty("maximumDrawdown").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies a later new high resets current drawdown while preserving the historical maximum drawdown.
    /// FA: بررسی می‌کند قله جدید بعدی افت فعلی را صفر کند ولی بیشینه افت تاریخی حفظ شود.
    /// </summary>
    [Fact]
    public async Task Drawdown_Should_Ignore_External_Withdrawal_And_Deposit()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId = await CreateCurrencyAsync("CHF", $"Drawdown CHF {Guid.NewGuid():N}");
        string investorId = await CreateInvestorAsync($"Drawdown Flow Owner {Guid.NewGuid():N}");
        string portfolioId = await CreatePortfolioAsync(investorId, $"Drawdown Flow {Guid.NewGuid():N}");

        DateTimeOffset from = new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);
        DateTimeOffset withdrawalTime = from.AddDays(1);
        DateTimeOffset to = from.AddDays(2);

        await SetBaseCurrencyAsync(portfolioId, currencyId);
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 1000m, from.AddDays(-1), null));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 2, 200m, withdrawalTime, "Withdrawal"));
        await CreateCashAsync(new CashRequest(portfolioId, currencyId, 1, 400m, to, "Deposit"));

        using HttpResponseMessage response =
            await GetDrawdownAsync(portfolioId, from, to, "Daily");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using JsonDocument document = await ReadJsonAsync(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("isComplete").GetBoolean());
        Assert.Equal(0m, root.GetProperty("currentDrawdown").GetDecimal());
        Assert.Equal(0m, root.GetProperty("maximumDrawdown").GetDecimal());
        Assert.Equal(1m, root.GetProperty("peakWealthIndex").GetDecimal());
    }

    private async Task<HttpResponseMessage> GetDrawdownAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/drawdown" +
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
