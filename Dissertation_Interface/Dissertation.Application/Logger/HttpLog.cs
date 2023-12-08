
namespace Dissertation.Application.Logger;

public class HttpLog
{
    public HttpLog(RequestLog request, ResponseLog response, int statusCode, DateTime timestamp, long duration,
        IReadOnlyDictionary<string, string>? metadata)
    {
        Request = request;
        Response = response;
        StatusCode = statusCode;
        Duration = duration;
        Timestamp = timestamp;
        Metadata = metadata;
    }

    #region Define Propperties

    public Guid Id { get; } = Guid.NewGuid();

    public RequestLog Request { get; }

    public ResponseLog Response { get; }

    public int StatusCode { get; }

    public long Duration { get; }

    public DateTime Timestamp { get; }

    public IReadOnlyDictionary<string, string>? Metadata { get; }

    #endregion
}

public record RequestLog(string Method, string Path, string? Body, string? ContentType, IReadOnlyDictionary<string, string?[]>? QueryString, IReadOnlyDictionary<string, string>? Headers);

public record ResponseLog(string Body, string? ContentType, IReadOnlyDictionary<string, string>? Headers);