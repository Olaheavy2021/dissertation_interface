using Shared.Enums;

namespace Dissertation.Domain.Interfaces;

public interface IRequestHelper
{
    Task<string> PostAsync(string url, object payload, IDictionary<string, object>? queryParams = null,
        IDictionary<string, string>? headers = null, Shared.Enums.MediaType? mediaType = null);

    Task<string> GetAsync(string url, object? payload, IDictionary<string, object>? queryParams = null,
        IDictionary<string, string>? headers = null, Shared.Enums.MediaType? mediaType = null);
    Task<string> DeleteAsync(string url, object? payload, IDictionary<string, object>? queryParams = null,
        IDictionary<string, string>? headers = null, MediaType? mediaType = null);
}