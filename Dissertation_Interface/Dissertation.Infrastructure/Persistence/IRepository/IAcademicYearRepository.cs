using Dissertation.Domain.Entities;
using Dissertation.Domain.Pagination;
using Shared.DTO;
using Shared.Helpers;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.IRepository;

public interface IAcademicYearRepository : IGenericRepository<AcademicYear>
{
    Task<bool> IsAcademicYearUnique(DateTime startDate, DateTime endDate);

    Task<AcademicYear?> GetActiveAcademicYear();

    PagedList<AcademicYear> GetListOfAcademicYears(AcademicYearPaginationParameters paginationParameters);

}