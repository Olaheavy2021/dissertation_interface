using Shared.DTO;

namespace Dissertation.Domain.Interfaces;

public interface IDissertationMatchingService
{
    Task<InitiateMatchingResponse> ProcessData(InitiateMatchingRequest request);

    Task<MatchingStatusRootObject> CheckStatus(string taskId);
}