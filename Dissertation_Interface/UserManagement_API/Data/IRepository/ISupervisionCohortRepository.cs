using Shared.DTO;
using Shared.Helpers;
using Shared.Repository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.IRepository;

public interface ISupervisionCohortRepository : IGenericRepository<SupervisionCohort>
{
    PagedList<ApplicationUser> GetActiveSupervisors(SupervisionCohortListParameters listParameters);

    PagedList<ApplicationUser> GetInActiveSupervisors(SupervisionCohortListParameters listParameters);

    PagedList<SupervisionCohort> GetPaginatedListOfSupervisionCohort(SupervisionCohortListParameters listParameters);
}