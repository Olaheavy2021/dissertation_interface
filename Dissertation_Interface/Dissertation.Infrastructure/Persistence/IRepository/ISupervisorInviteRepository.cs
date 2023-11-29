using Dissertation.Domain.Entities;
using Dissertation.Domain.Pagination;
using Shared.Helpers;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.IRepository;

public interface ISupervisorInviteRepository : IGenericRepository<SupervisorInvite>
{
    PagedList<SupervisorInvite> GetListOfSupervisorInvites(SupervisorInvitePaginationParameters paginationParameters);
}