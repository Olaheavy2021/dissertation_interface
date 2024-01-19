using System.Diagnostics.CodeAnalysis;

namespace Notification_API.Middleware.Correlation;

[ExcludeFromCodeCoverage]
public class CorrelationIdGenerator : ICorrelationIdGenerator
{
    private string _correlationId = Guid.NewGuid().ToString("D");
    public string Get() => this._correlationId;

    public void Set(string correlationId) => this._correlationId = correlationId;
}