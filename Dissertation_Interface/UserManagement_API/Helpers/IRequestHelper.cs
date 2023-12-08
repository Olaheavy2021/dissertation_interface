namespace UserManagement_API.Helpers;

public interface IRequestHelper
{
    Task<string> PostAsync(string url, object payload, IDictionary<string, object>? queryParams = null,
        IDictionary<string, string>? headers = null, Shared.Enums.MediaType? mediaType = null);

    Task<string> GetAsync(string url, object? payload, IDictionary<string, object>? queryParams = null,
        IDictionary<string, string>? headers = null, Shared.Enums.MediaType? mediaType = null);
}