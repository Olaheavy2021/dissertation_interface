using Dissertation.Domain.Entities;
using Dissertation.Domain.Pagination;
using Shared.Helpers;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.IRepository;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    PagedList<Department> GetListOfDepartments(DepartmentPaginationParameters paginationParameters);

}