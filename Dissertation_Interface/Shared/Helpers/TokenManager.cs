using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Shared.Constants;
using Shared.DTO;
using Shared.Settings;

namespace Shared.Helpers;

public class TokenManager : ITokenManager
{
    private readonly IRedisCacheHelper _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtSettings _jwtOptions;

    public TokenManager(IRedisCacheHelper cache,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtSettings> jwtOptions
    )
    {
        this._cache = cache;
        this._httpContextAccessor = httpContextAccessor;
        this._jwtOptions = jwtOptions.Value;
    }

    public async Task<bool> IsCurrentActiveToken()
    {
        var token = GetCurrentAsync();
        if (!string.IsNullOrEmpty(token))
        {
            return await IsActiveAsync(token);
        }

        return true;
    }


    public async Task<ResponseDto<string>> DeactivateCurrentAsync()
    {
        var response = new ResponseDto<string>();
        var token = GetCurrentAsync();
        if (!string.IsNullOrEmpty(token))
        {
            await DeactivateAsync(token);
            response.Message = SuccessMessages.DefaultSuccess;
            response.IsSuccess = true;
            response.Result = "User has been logged out successfully";
            return response;
        }

        response.IsSuccess = false;
        response.Message = ErrorMessages.DefaultError;
        response.Result = ErrorMessages.DefaultError;
        return response;
    }

    public async Task<bool> IsActiveAsync(string token)
        => await this._cache.GetCacheDataAsync<string>(GetKey(token)) == null;

    public async Task DeactivateAsync(string token)
        => await this._cache.SetCacheDataAsync(GetKey(token), token, this._jwtOptions.DurationInMinutes);

    private string? GetCurrentAsync()
    {
        StringValues authorizationHeader = this._httpContextAccessor
            .HttpContext!.Request.Headers["Authorization"];

        return authorizationHeader == StringValues.Empty
            ? string.Empty
            : authorizationHeader.Single()?.Split(" ").Last();
    }

    private static string GetKey(string token)
        => $"tokens:{token}:deactivated";
}