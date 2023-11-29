using Dissertation.Domain.Entities;
using Dissertation.Domain.Pagination;
using Shared.Helpers;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.IRepository;

public interface IDissertationCohortRepository : IGenericRepository<DissertationCohort>
{
    PagedList<DissertationCohort> GetListOfDissertationCohort(
        DissertationCohortPaginationParameters paginationParameters);

    Task<DissertationCohort?> GetActiveDissertationCohort();
}