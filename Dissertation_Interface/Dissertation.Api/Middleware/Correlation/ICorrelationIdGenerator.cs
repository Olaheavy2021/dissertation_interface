namespace Dissertation_API.Middleware.Correlation;

public interface ICorrelationIdGenerator
{
    string Get();
    void Set(string correlationId);
}