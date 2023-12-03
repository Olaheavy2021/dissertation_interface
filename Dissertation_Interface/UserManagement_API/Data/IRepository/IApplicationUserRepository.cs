using Shared.DTO;
using Shared.Helpers;
using Shared.Repository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.IRepository;

public interface IApplicationUserRepository : IGenericRepository<ApplicationUser>
{
    PagedList<ApplicationUser> GetPaginatedAdminUsers(UserPaginationParameters paginationParameters);

    PagedList<ApplicationUser> GetPaginatedStudents(DissertationStudentPaginationParameters paginationParameters);

    PagedList<ApplicationUser> GetPaginatedSupervisors(SupervisorPaginationParameters paginationParameters);

    Task<bool> DoesUserNameExist(string username, CancellationToken token);

    Task<bool> DoesEmailExist(string? email, CancellationToken token);
}