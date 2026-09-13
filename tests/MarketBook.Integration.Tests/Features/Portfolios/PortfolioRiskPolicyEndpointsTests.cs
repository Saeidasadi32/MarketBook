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

/// <summary>EN: Integration tests for persisted portfolio risk policies. FA: تست‌های Integration Policy ریسک ذخیره‌شده.</summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioRiskPolicyEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>EN: Initializes tests. FA: تست‌ها را مقداردهی می‌کند.</summary>
    public PortfolioRiskPolicyEndpointsTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>EN: First policy is version one and active. FA: اولین Policy نسخه یک و فعال است.</summary>
    [Fact]
    public async Task CreatePolicy_Should_Create_First_Active_Version()
    {
        string portfolioId = await CreatePortfolioAsync();

        using HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                $"/api/v1/portfolios/{portfolioId}/risk-policy",
                Limits(new DateTimeOffset(2026, 9, 13, 0, 0, 0, TimeSpan.Zero), 0.25m));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument document = await ReadJsonAsync(response);
        Assert.Equal(1, document.RootElement.GetProperty("policyVersion").GetInt32());
        Assert.Equal("Active", document.RootElement.GetProperty("status").GetString());
    }

    /// <summary>EN: Activating version two archives version one. FA: فعال‌سازی نسخه دو، نسخه یک را بایگانی می‌کند.</summary>
    [Fact]
    public async Task ActivatePolicy_Should_Archive_Previous_Active()
    {
        string portfolioId = await CreatePortfolioAsync();
        DateTimeOffset first = new(2026, 9, 13, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset second = first.AddDays(1);

        await _client.PostAsJsonAsync($"/api/v1/portfolios/{portfolioId}/risk-policy", Limits(first, 0.25m));
        await _client.PutAsJsonAsync($"/api/v1/portfolios/{portfolioId}/risk-policy", Limits(second, 0.20m));

        using HttpResponseMessage activate =
            await _client.PostAsync(
                $"/api/v1/portfolios/{portfolioId}/risk-policy/2/activate?effectiveFrom={Uri.EscapeDataString(second.ToString("O"))}",
                null);

        Assert.Equal(HttpStatusCode.OK, activate.StatusCode);

        using HttpResponseMessage history =
            await _client.GetAsync($"/api/v1/portfolios/{portfolioId}/risk-policy/history");

        using JsonDocument document = await ReadJsonAsync(history);
        JsonElement[] items = document.RootElement.EnumerateArray().ToArray();
        Assert.Equal("Archived", items[0].GetProperty("status").GetString());
        Assert.Equal("Active", items[1].GetProperty("status").GetString());
    }

    /// <summary>EN: History is returned in business-version order. FA: تاریخچه بر اساس نسخه کسب‌وکاری مرتب است.</summary>
    [Fact]
    public async Task History_Should_Return_Versions_In_Order()
    {
        string portfolioId = await CreatePortfolioAsync();
        DateTimeOffset first = new(2026, 9, 13, 0, 0, 0, TimeSpan.Zero);

        await _client.PostAsJsonAsync($"/api/v1/portfolios/{portfolioId}/risk-policy", Limits(first, 0.30m));
        await _client.PutAsJsonAsync($"/api/v1/portfolios/{portfolioId}/risk-policy", Limits(first.AddDays(1), 0.25m));
        await _client.PutAsJsonAsync($"/api/v1/portfolios/{portfolioId}/risk-policy", Limits(first.AddDays(2), 0.20m));

        using HttpResponseMessage history =
            await _client.GetAsync($"/api/v1/portfolios/{portfolioId}/risk-policy/history");

        Assert.Equal(HttpStatusCode.OK, history.StatusCode);
        using JsonDocument document = await ReadJsonAsync(history);
        int[] versions = document.RootElement.EnumerateArray()
            .Select(item => item.GetProperty("policyVersion").GetInt32())
            .ToArray();

        Assert.Equal(new[] { 1, 2, 3 }, versions);
    }

    private async Task<string> CreatePortfolioAsync()
    {
        await _fixture.ResetInvestorsAsync();

        using HttpResponseMessage investor =
            await _client.PostAsJsonAsync(
                "/api/v1/investors",
                new { fullName = $"Risk Policy Owner {Guid.NewGuid():N}" });

        using JsonDocument investorJson = await ReadJsonAsync(investor);
        string investorId = investorJson.RootElement.GetProperty("id").GetString()!;

        using HttpResponseMessage portfolio =
            await _client.PostAsJsonAsync(
                "/api/v1/portfolios",
                new { investorId, name = $"Risk Policy {Guid.NewGuid():N}" });

        Assert.Equal(HttpStatusCode.Created, portfolio.StatusCode);
        using JsonDocument portfolioJson = await ReadJsonAsync(portfolio);
        return portfolioJson.RootElement.GetProperty("id").GetString()!;
    }

    private static object Limits(DateTimeOffset effectiveFrom, decimal maxAnnualizedVolatility)
        => new
        {
            effectiveFrom,
            maxAnnualizedVolatility,
            maxValueAtRiskReturn = (decimal?)null,
            maxValueAtRiskAmountBase = (decimal?)null,
            maxDrawdownLossRatio = (decimal?)null,
            maxDrawdownAmountBase = (decimal?)null,
            minSharpeRatio = (decimal?)null,
            minSortinoRatio = (decimal?)null
        };

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        => await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
}
