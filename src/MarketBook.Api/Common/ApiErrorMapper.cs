// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : API
// Namespace : MarketBook.Api.Common
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace MarketBook.Api.Common;

/// <summary>
/// EN: Maps domain errors to HTTP API responses.
/// FA: خطاهای دامنه را به پاسخ‌های HTTP مناسب برای API نگاشت می‌کند.
/// </summary>
internal static class ApiErrorMapper
{
    /// <summary>
    /// EN: Converts a domain error into an HTTP action result.
    /// FA: یک خطای دامنه را به نتیجه مناسب HTTP تبدیل می‌کند.
    /// </summary>
    public static IActionResult ToActionResult(
        ControllerBase controller,
        Error error)
    {
        ArgumentNullException.ThrowIfNull(controller);
        ArgumentNullException.ThrowIfNull(error);

        int statusCode = error.Code switch
        {
            "Exchange.CountryNotFound" =>
                StatusCodes.Status404NotFound,

            _ when error.Code.EndsWith(
                ".NotFound",
                StringComparison.Ordinal) =>
                StatusCodes.Status404NotFound,

            _ when error.Code.Contains(
                ".Duplicate",
                StringComparison.Ordinal) ||
                error.Code.Contains(
                    ".AlreadyExists",
                    StringComparison.Ordinal) ||
                error.Code.Contains(
                    ".Conflict",
                    StringComparison.Ordinal) =>
                StatusCodes.Status409Conflict,

            _ =>
                StatusCodes.Status400BadRequest
        };

        ProblemDetails problemDetails = new()
        {
            Status = statusCode,
            Title = GetTitle(statusCode),
            Detail = error.Message,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        problemDetails.Extensions["code"] = error.Code;

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// EN: Gets the standard HTTP title for the specified status code.
    /// FA: عنوان استاندارد HTTP متناظر با کد وضعیت HTTP را برمی‌گرداند.
    /// </summary>
    private static string GetTitle(int statusCode)
        => statusCode switch
        {
            StatusCodes.Status400BadRequest =>
                "Bad Request",

            StatusCodes.Status404NotFound =>
                "Not Found",

            StatusCodes.Status409Conflict =>
                "Conflict",

            _ =>
                "Error"
        };
}
