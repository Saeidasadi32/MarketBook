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
/// EN: Integration tests for base-currency historical VaR/CVaR amounts.
/// FA: تست‌های Integration مربوط به مبالغ VaR/CVaR تاریخی در ارز پایه.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public sealed class PortfolioValueAtRiskAmountEndpointsTests
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    /// <summary>
    /// EN: Initializes historical VaR/CVaR amount endpoint tests.
    /// FA: تست‌های Endpoint مربوط به VaR/CVaR مبلغی تاریخی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="fixture">EN: Integration fixture. FA: Fixture تست Integration.</param>
    public PortfolioValueAtRiskAmountEndpointsTests(IntegrationTestFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
        _client = fixture.Client;
    }

    /// <summary>
    /// EN: Verifies a 10% historical loss ratio is translated using the exact ending NAV.
    /// FA: بررسی می‌کند نسبت زیان تاریخی ۱۰٪ با NAV دقیق پایانی به مبلغ تبدیل شود.
    /// </summary>
    [Fact]
    public async Task ValueAtRiskAmount_Should_Use_Exact_Ending_Nav()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "SEK",
                $"VaR Amount SEK {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"VaR Amount Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"VaR Amount {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

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
                7,
                100m,
                from.AddDays(1),
                "Dividend"));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                5,
                110m,
                to,
                "Fee"));

        using HttpResponseMessage response =
            await GetValueAtRiskAmountAsync(
                portfolioId,
                from,
                to,
                "Daily",
                0.95m);

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
            root.GetProperty("isCalculable").GetBoolean());

        Assert.Equal(
            990m,
            root.GetProperty("netAssetValueBase").GetDecimal());

        Assert.Equal(
            0.10m,
            root.GetProperty("valueAtRiskReturn").GetDecimal());

        Assert.Equal(
            0.10m,
            root.GetProperty("conditionalValueAtRiskReturn").GetDecimal());

        Assert.Equal(
            99m,
            root.GetProperty("valueAtRiskAmountBase").GetDecimal());

        Assert.Equal(
            99m,
            root.GetProperty("conditionalValueAtRiskAmountBase").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies positive-return historical tails produce zero loss amounts, not negative risk amounts.
    /// FA: بررسی می‌کند دنباله کاملاً مثبت مبلغ زیان صفر تولید کند و مبلغ ریسک منفی ساخته نشود.
    /// </summary>
    [Fact]
    public async Task ValueAtRiskAmount_Should_Return_Zero_For_All_Positive_Tail()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "NOK",
                $"VaR Amount NOK {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"VaR Amount Positive Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"VaR Amount Positive {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

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
                7,
                100m,
                from.AddDays(1),
                "Dividend1"));

        await CreateCashAsync(
            new CashRequest(
                portfolioId,
                currencyId,
                7,
                55m,
                to,
                "Dividend2"));

        using HttpResponseMessage response =
            await GetValueAtRiskAmountAsync(
                portfolioId,
                from,
                to,
                "Daily",
                0.95m);

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
            root.GetProperty("isCalculable").GetBoolean());

        Assert.Equal(
            1155m,
            root.GetProperty("netAssetValueBase").GetDecimal());

        Assert.Equal(
            0m,
            root.GetProperty("valueAtRiskReturn").GetDecimal());

        Assert.Equal(
            0m,
            root.GetProperty("conditionalValueAtRiskReturn").GetDecimal());

        Assert.Equal(
            0m,
            root.GetProperty("valueAtRiskAmountBase").GetDecimal());

        Assert.Equal(
            0m,
            root.GetProperty("conditionalValueAtRiskAmountBase").GetDecimal());
    }

    /// <summary>
    /// EN: Verifies invalid confidence levels are rejected through the existing DOC-0037 validation contract.
    /// FA: بررسی می‌کند سطح اطمینان نامعتبر از قرارداد اعتبارسنجی موجود DOC-0037 رد شود.
    /// </summary>
    [Fact]
    public async Task ValueAtRiskAmount_Should_Reject_Invalid_Confidence_Level()
    {
        await _fixture.ResetPortfolioTransactionsAsync();

        string currencyId =
            await CreateCurrencyAsync(
                "DKK",
                $"VaR Amount DKK {Guid.NewGuid():N}");

        string investorId =
            await CreateInvestorAsync(
                $"VaR Amount Invalid Owner {Guid.NewGuid():N}");

        string portfolioId =
            await CreatePortfolioAsync(
                investorId,
                $"VaR Amount Invalid {Guid.NewGuid():N}");

        DateTimeOffset from =
            new(2026, 9, 8, 12, 0, 0, TimeSpan.Zero);

        DateTimeOffset to =
            from.AddDays(2);

        await SetBaseCurrencyAsync(
            portfolioId,
            currencyId);

        using HttpResponseMessage response =
            await GetValueAtRiskAmountAsync(
                portfolioId,
                from,
                to,
                "Daily",
                1.00m);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private async Task<HttpResponseMessage> GetValueAtRiskAmountAsync(
        string portfolioId,
        DateTimeOffset from,
        DateTimeOffset to,
        string interval,
        decimal confidenceLevel)
        => await _client.GetAsync(
            $"/api/v1/portfolios/{portfolioId}/performance/risk/var/amount" +
            $"?from={Uri.EscapeDataString(from.ToString("O"))}" +
            $"&to={Uri.EscapeDataString(to.ToString("O"))}" +
            $"&interval={Uri.EscapeDataString(interval)}" +
            $"&confidenceLevel={confidenceLevel.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

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
