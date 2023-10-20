using System.Diagnostics;

namespace Shared.DTO;

public class ApplicationProblemDetails
{
    public string? Name { get; init; }

    public string? DebugId { get; } = Activity.Current?.Id;

    public string? Message { get; set; }

    public string? InformationLink { get; set; }

    public ApplicationProblemDetails(int statusCode)
    {
        if (this._defaults.TryGetValue(statusCode, out var clientErrorData))
        {
            Name ??= clientErrorData.Title;
            InformationLink ??= clientErrorData.Type;
        }
    }

    private readonly Dictionary<int, (string Type, string Title)> _defaults = new()
    {
        [400] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            "Bad Request"
        ),
        [401] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            "Unauthorized"
        ),
        [403] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            "Forbidden"
        ),
        [404] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            "Not Found"
        ),
        [405] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.6",
            "Method Not Allowed"
        ),
        [406] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.7",
            "Not Acceptable"
        ),
        [409] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.10",
            "Conflict"
        ),
        [415] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.16",
            "Unsupported Media Type"
        ),
        [422] =
        (
            "https://tools.ietf.org/html/rfc4918#section-11.2",
            "Unprocessable Entity"
        ),
        [500] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            "An error occurred while processing your request."
        ),
    };
}