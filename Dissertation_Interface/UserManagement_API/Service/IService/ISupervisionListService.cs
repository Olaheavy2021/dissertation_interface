using Shared.DTO;

namespace UserManagement_API.Service.IService;

public interface ISupervisionListService
{
    Task<ResponseDto<PaginatedSupervisionListDto>> GetPaginatedListOfSupervisionRequest(
        SupervisionListPaginationParameters parameters);

    Task<ResponseDto<PaginatedSupervisionListDto>> GetPaginatedListOfSupervisionListForAStudent(
        SupervisionListPaginationParameters parameters);
    Task<ResponseDto<PaginatedSupervisionListDto>> GetPaginatedListOfSupervisionListForASupervisor(
        SupervisionListPaginationParameters parameters);
}