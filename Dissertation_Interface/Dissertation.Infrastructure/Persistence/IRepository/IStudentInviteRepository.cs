using Dissertation.Domain.Entities;
using Dissertation.Domain.Pagination;
using Shared.Helpers;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.IRepository;

public interface IStudentInviteRepository : IGenericRepository<StudentInvite>
{
    PagedList<StudentInvite> GetListOfStudentInvites(StudentInvitePaginationParameters paginationParameters,
        long cohortId);
}