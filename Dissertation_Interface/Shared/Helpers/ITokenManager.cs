using Shared.DTO;

namespace Shared.Helpers;

public interface ITokenManager
{
    Task<bool> IsCurrentActiveToken();
    Task<ResponseDto<string>> DeactivateCurrentAsync();
    Task<bool> IsActiveAsync(string token);
    Task DeactivateAsync(string token);
}