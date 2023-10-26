namespace Dissertation_API.Middleware.Correlation;

public class CorrelationIdGenerator : ICorrelationIdGenerator
{
    private string _correlationId = Guid.NewGuid().ToString("D");
    public string Get() => this._correlationId;

    public void Set(string correlationId) => this._correlationId = correlationId;
}