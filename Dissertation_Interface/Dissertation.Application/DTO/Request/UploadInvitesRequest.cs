using Dissertation.Infrastructure.DTO;

namespace Dissertation.Application.DTO.Request;

public class UploadInvitesRequest
{
    public List<UserUploadRequest> Requests { get; set; } = null!;
}