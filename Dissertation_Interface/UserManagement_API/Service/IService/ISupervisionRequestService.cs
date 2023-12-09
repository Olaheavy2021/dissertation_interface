using Shared.DTO;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Service.IService;

public interface ISupervisionRequestService
{
    Task<ResponseDto<string>> CreateSupervisionRequest(CreateSupervisionRequest request, CancellationToken cancellationToken);
}