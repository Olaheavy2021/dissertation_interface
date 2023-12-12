using Shared.DTO;
using Shared.Helpers;
using Shared.Repository;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Data.IRepository;

public interface ISupervisionRequestRepository : IGenericRepository<SupervisionRequest>
{
    PagedList<SupervisionRequest> GetPaginatedListOfSupervisionRequests(
        SupervisionRequestPaginationParameters parameters);

    PagedList<SupervisionRequest>
        GetStudentListOfSupervisionRequests(SupervisionRequestPaginationParameters parameters);

    PagedList<SupervisionRequest> GetSupervisorListOfSupervisionRequests(
        SupervisionRequestPaginationParameters parameters);
}