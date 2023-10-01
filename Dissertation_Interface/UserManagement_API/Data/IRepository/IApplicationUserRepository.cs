using Shared.Helpers;
using Shared.Repository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.IRepository;

public interface IApplicationUserRepository : IGenericRepository<ApplicationUser>
{
    PagedList<ApplicationUser> GetPaginatedAdminUsers(PaginationParameters paginationParameters);

    Task<bool> DoesUserNameExist(string username, CancellationToken token);

    Task<bool> DoesEmailExist(string email, CancellationToken token);
}