// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : API
// Namespace : MarketBook.Api.Controllers
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------


using MarketBook.Api.Common;
using MarketBook.Application.Features.Portfolios.Commands.ActivatePortfolio;
using MarketBook.Application.Features.Portfolios.Commands.CreatePortfolio;
using MarketBook.Application.Features.Portfolios.Commands.DeactivatePortfolio;
using MarketBook.Application.Features.Portfolios.Commands.SetPortfolioBaseCurrency;
using MarketBook.Application.Features.Portfolios.Commands.UpdatePortfolio;
using MarketBook.Application.Features.Portfolios.Queries.GetAllPortfolios;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioById;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNav;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioNavAsOf;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNav;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformance;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTimeWeightedReturn;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMoneyWeightedReturn;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceComparison;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformancePresets;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceSeries;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdown;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdownEpisodes;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskStatistics;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRiskRatios;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRisk;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRisk;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Controllers;

/// <summary>
/// EN: Provides Portfolio HTTP endpoints.
/// FA: Endpointهای HTTP پرتفوی را فراهم می‌کند.
/// </summary>
[ApiController]
[Route("api/v1/portfolios")]
public sealed class PortfoliosController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the controller.
    /// FA: Controller را مقداردهی می‌کند.
    /// </summary>
    public PortfoliosController(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Creates a portfolio.
    /// FA: یک پرتفوی ایجاد می‌کند.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePortfolioRequest request,
        CancellationToken cancellationToken)
    {
        Result<PortfolioId> result = await _sender.Send(
            new CreatePortfolioCommand(request.InvestorId, request.Name),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Value.ToString() },
                new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Renames a portfolio.
    /// FA: نام پرتفوی را تغییر می‌دهد.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdatePortfolioRequest request,
        CancellationToken cancellationToken)
    {
        Result<PortfolioId> result = await _sender.Send(
            new UpdatePortfolioCommand(id, request.Name),
            cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Sets the reporting/base currency of a portfolio.
    /// FA: ارز پایه/گزارش‌دهی یک پرتفوی را تنظیم می‌کند.
    /// </summary>
    [HttpPatch("{id}/base-currency")]
    public async Task<IActionResult> SetBaseCurrency(
        string id,
        [FromBody] SetPortfolioBaseCurrencyRequest request,
        CancellationToken cancellationToken)
    {
        Result<PortfolioId> result =
            await _sender.Send(
                new SetPortfolioBaseCurrencyCommand(
                    id,
                    request.CurrencyId),
                cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Activates a portfolio.
    /// FA: پرتفوی را فعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(
        string id,
        CancellationToken cancellationToken)
    {
        Result<PortfolioId> result =
            await _sender.Send(new ActivatePortfolioCommand(id), cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Deactivates a portfolio.
    /// FA: پرتفوی را غیرفعال می‌کند.
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(
        string id,
        CancellationToken cancellationToken)
    {
        Result<PortfolioId> result =
            await _sender.Send(new DeactivatePortfolioCommand(id), cancellationToken);

        return result.IsSuccess
            ? Ok(new { id = result.Value!.Value.ToString() })
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets a portfolio by identifier.
    /// FA: پرتفوی را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioByIdResponse> result =
            await _sender.Send(new GetPortfolioByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets current net asset value grouped by currency.
    /// FA: ارزش خالص دارایی جاری را به تفکیک ارز دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/nav")]
    public async Task<IActionResult> GetNav(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioNavResponse> result =
            await _sender.Send(
                new GetPortfolioNavQuery(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets portfolio NAV reconstructed at an inclusive historical cutoff instant.
    /// FA: NAV پرتفوی را در یک لحظه تاریخی شامل‌شونده بازسازی و دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/nav/as-of")]
    public async Task<IActionResult> GetNavAsOf(
        string id,
        [FromQuery] DateTimeOffset asOf,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioNavAsOfResponse> result =
            await _sender.Send(
                new GetPortfolioNavAsOfQuery(id, asOf),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets current portfolio NAV translated into the configured base currency.
    /// FA: NAV جاری پرتفوی را پس از ترجمه به ارز پایه تنظیم‌شده دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/translated-nav")]
    public async Task<IActionResult> GetTranslatedNav(
        string id,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioTranslatedNavResponse> result =
            await _sender.Send(
                new GetPortfolioTranslatedNavQuery(id),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets historical portfolio NAV translated into the configured base currency using FX available on or before the as-of date.
    /// FA: NAV تاریخی پرتفوی را با استفاده از نرخ FX موجود در تاریخ As-Of یا قبل از آن به ارز پایه تنظیم‌شده ترجمه می‌کند.
    /// </summary>
    [HttpGet("{id}/translated-nav/as-of")]
    public async Task<IActionResult> GetTranslatedNavAsOf(
        string id,
        [FromQuery] DateTimeOffset asOf,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioTranslatedNavAsOfResponse> result =
            await _sender.Send(
                new GetPortfolioTranslatedNavAsOfQuery(id, asOf),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets cash-flow-aware historical portfolio performance foundation in the configured base currency.
    /// FA: مبنای عملکرد تاریخی پرتفوی را با لحاظ جریان نقدی خارجی و در ارز پایه تنظیم‌شده دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/performance")]
    public async Task<IActionResult> GetPerformance(
        string id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioPerformanceResponse> result =
            await _sender.Send(
                new GetPortfolioPerformanceQuery(id, from, to),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets historical portfolio time-weighted return with external cash-flow neutralization.
    /// FA: بازده زمانی‌وزن تاریخی پرتفوی را با خنثی‌سازی جریان‌های نقدی خارجی دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/twr")]
    public async Task<IActionResult> GetTimeWeightedReturn(
        string id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioTimeWeightedReturnResponse> result =
            await _sender.Send(
                new GetPortfolioTimeWeightedReturnQuery(id, from, to),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets historical money-weighted return (XIRR) from dated external cash flows and terminal NAV.
    /// FA: بازده پول‌وزن تاریخی (XIRR) را از جریان‌های نقدی خارجی تاریخ‌دار و NAV نهایی دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/xirr")]
    public async Task<IActionResult> GetMoneyWeightedReturn(
        string id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioMoneyWeightedReturnResponse> result =
            await _sender.Send(
                new GetPortfolioMoneyWeightedReturnQuery(id, from, to),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Compares historical time-weighted and money-weighted portfolio returns for one period.
    /// FA: بازده تاریخی زمان‌وزن و پول‌وزن پرتفوی را برای یک دوره مقایسه می‌کند.
    /// </summary>
    [HttpGet("{id}/performance/comparison")]
    public async Task<IActionResult> GetPerformanceComparison(
        string id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioPerformanceComparisonResponse> result =
            await _sender.Send(
                new GetPortfolioPerformanceComparisonQuery(id, from, to),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets standard dashboard performance periods ending at the supplied as-of instant.
    /// FA: دوره‌های استاندارد عملکرد داشبورد را تا لحظه as-of داده‌شده دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/performance/presets")]
    public async Task<IActionResult> GetPerformancePresets(
        string id,
        [FromQuery] DateTimeOffset asOf,
        CancellationToken cancellationToken)
    {
        Result<GetPortfolioPerformancePresetsResponse> result =
            await _sender.Send(
                new GetPortfolioPerformancePresetsQuery(id, asOf),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets historical chart-series data with base-currency NAV, cumulative TWR, and external-flow markers.
    /// FA: داده سری زمانی تاریخی شامل NAV ارز پایه، TWR تجمعی و markerهای جریان خارجی را دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/performance/series")]
    public async Task<IActionResult> GetPerformanceSeries(
        string id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        [FromQuery] string interval = "Daily",
        CancellationToken cancellationToken = default)
    {
        Result<GetPortfolioPerformanceSeriesResponse> result =
            await _sender.Send(
                new GetPortfolioPerformanceSeriesQuery(id, from, to, interval),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets portfolio drawdown analytics from historical base-currency NAV.
    /// FA: تحلیل افت سرمایه پرتفوی را از NAV تاریخی ارز پایه دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/performance/drawdown")]
    public async Task<IActionResult> GetDrawdown(
        string id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        [FromQuery] string interval = "Daily",
        CancellationToken cancellationToken = default)
    {
        Result<GetPortfolioDrawdownResponse> result =
            await _sender.Send(
                new GetPortfolioDrawdownQuery(id, from, to, interval),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets drawdown-duration and recovery episodes for a portfolio.
    /// FA: دوره‌های مدت افت و بازیابی پرتفوی را دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/performance/drawdown/episodes")]
    public async Task<IActionResult> GetDrawdownEpisodes(
        string id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        [FromQuery] string interval = "Daily",
        CancellationToken cancellationToken = default)
    {
        Result<GetPortfolioDrawdownEpisodesResponse> result =
            await _sender.Send(
                new GetPortfolioDrawdownEpisodesQuery(id, from, to, interval),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets volatility and downside-deviation statistics from periodic TWR returns.
    /// FA: آمار نوسان و انحراف نزولی را از بازده‌های دوره‌ای TWR دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/performance/risk-statistics")]
    public async Task<IActionResult> GetRiskStatistics(
        string id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        [FromQuery] string interval = "Daily",
        CancellationToken cancellationToken = default)
    {
        Result<GetPortfolioRiskStatisticsResponse> result =
            await _sender.Send(
                new GetPortfolioRiskStatisticsQuery(id, from, to, interval),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets Sharpe and Sortino ratios with optional annual risk-free and minimum acceptable rates.
    /// FA: نسبت‌های Sharpe و Sortino را با نرخ بدون‌ریسک و حداقل بازده قابل‌قبول سالانه اختیاری دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/performance/risk-ratios")]
    public async Task<IActionResult> GetRiskRatios(
        string id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        [FromQuery] string interval = "Daily",
        [FromQuery] decimal riskFreeRateAnnual = 0m,
        [FromQuery] decimal minimumAcceptableReturnAnnual = 0m,
        CancellationToken cancellationToken = default)
    {
        Result<GetPortfolioRiskRatiosResponse> result =
            await _sender.Send(
                new GetPortfolioRiskRatiosQuery(
                    id,
                    from,
                    to,
                    interval,
                    riskFreeRateAnnual,
                    minimumAcceptableReturnAnnual),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets rolling volatility, downside deviation, Sharpe, and Sortino analytics.
    /// FA: تحلیل‌های Rolling نوسان، انحراف نزولی، Sharpe و Sortino را دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/performance/risk/rolling")]
    public async Task<IActionResult> GetRollingRisk(
        string id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        [FromQuery] string interval = "Daily",
        [FromQuery] int windowPeriods = 30,
        [FromQuery] decimal riskFreeRateAnnual = 0m,
        [FromQuery] decimal minimumAcceptableReturnAnnual = 0m,
        CancellationToken cancellationToken = default)
    {
        Result<GetPortfolioRollingRiskResponse> result =
            await _sender.Send(
                new GetPortfolioRollingRiskQuery(
                    id,
                    from,
                    to,
                    interval,
                    windowPeriods,
                    riskFreeRateAnnual,
                    minimumAcceptableReturnAnnual),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets historical Value at Risk and Conditional Value at Risk from periodic portfolio returns.
    /// FA: VaR و CVaR تاریخی را از بازده‌های دوره‌ای پرتفوی دریافت می‌کند.
    /// </summary>
    [HttpGet("{id}/performance/risk/var")]
    public async Task<IActionResult> GetValueAtRisk(
        string id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        [FromQuery] string interval = "Daily",
        [FromQuery] decimal confidenceLevel = 0.95m,
        CancellationToken cancellationToken = default)
    {
        Result<GetPortfolioValueAtRiskResponse> result =
            await _sender.Send(
                new GetPortfolioValueAtRiskQuery(
                    id,
                    from,
                    to,
                    interval,
                    confidenceLevel),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }

    /// <summary>
    /// EN: Gets paged portfolios.
    /// FA: پرتفوی‌های صفحه‌بندی‌شده را دریافت می‌کند.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? investorId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<GetAllPortfoliosResponse> result =
            await _sender.Send(
                new GetAllPortfoliosQuery(page, pageSize, investorId),
                cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiErrorMapper.ToActionResult(this, result.Error);
    }
}
