using System.Diagnostics.CodeAnalysis;

namespace UserManagement_API.Extensions;

[ExcludeFromCodeCoverage]
public static class HttpContextExtensions
{
    public static string? GetUserId(this HttpContext context) => context.Items["UserId"] as string;

    public static string? GetEmail(this HttpContext context) => context.Items["Email"] as string;
}