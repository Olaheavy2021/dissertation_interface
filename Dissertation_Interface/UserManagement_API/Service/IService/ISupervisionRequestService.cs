using Shared.DTO;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Service.IService;

public interface ISupervisionRequestService
{
    Task<ResponseDto<string>> CreateSupervisionRequest(CreateSupervisionRequest request, CancellationToken cancellationToken);

    Task<ResponseDto<PaginatedSupervisionRequestListDto>> GetPaginatedListOfSupervisionRequest(
        SupervisionRequestPaginationParameters parameters);
    Task<ResponseDto<PaginatedSupervisionRequestListDto>> GetPaginatedListOfSupervisionRequestForAStudent(
        SupervisionRequestPaginationParameters parameters);
    Task<ResponseDto<PaginatedSupervisionRequestListDto>> GetPaginatedListOfSupervisionRequestForASupervisor(
        SupervisionRequestPaginationParameters parameters);
    Task<ResponseDto<string>> RejectSupervisionRequest(ActionSupervisionRequest request, CancellationToken cancellationToken);
    Task<ResponseDto<string>> AcceptSupervisionRequest(ActionSupervisionRequest request, CancellationToken cancellationToken);
    Task<ResponseDto<string>> CancelSupervisionRequest(ActionSupervisionRequest request, CancellationToken cancellationToken);


}