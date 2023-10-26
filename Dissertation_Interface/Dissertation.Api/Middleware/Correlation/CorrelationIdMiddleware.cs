using Microsoft.Extensions.Primitives;

namespace Dissertation_API.Middleware.Correlation;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "x-correlation-id";

    public CorrelationIdMiddleware(RequestDelegate next) => this._next = next;

    public async Task Invoke(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
    {
        StringValues correlationId = GetCorrelationId(context, correlationIdGenerator);
        AddCorrelationIdHeader(context, correlationId);
        await this._next(context);
    }

    private static StringValues GetCorrelationId(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out StringValues correlationId))
        {
            correlationIdGenerator.Set(correlationId);
            return correlationId;
        }

        return correlationIdGenerator.Get();
    }

    private static void AddCorrelationIdHeader(HttpContext context, StringValues correlationId) =>
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId.ToString();
            return Task.CompletedTask;
        });
}