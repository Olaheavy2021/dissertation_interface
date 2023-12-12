using Shared.DTO;
using Shared.Helpers;
using Shared.Repository;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Data.IRepository;

public interface ISupervisionListRepository : IGenericRepository<SupervisionList>
{
    PagedList<SupervisionList> GetPaginatedListOfSupervisionLists(
        SupervisionListPaginationParameters parameters);

    PagedList<SupervisionList> GetSupervisionListsForSupervisor(
        SupervisionListPaginationParameters parameters);

    PagedList<SupervisionList> GetSupervisionListsForStudent(
        SupervisionListPaginationParameters parameters);
}