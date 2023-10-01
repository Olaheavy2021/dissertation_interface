using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;
using Shared.Repository;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.Repository;

public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
{
    public ApplicationUserRepository(DbContext dbContext) : base(dbContext)
    {

    }

    public PagedList<ApplicationUser> GetPaginatedAdminUsers(PaginationParameters paginationParameters)
    {
        string[] roleNames = { "SuperAdmin", "Admin" };
        const string query = @"SELECT U.*, R.Name FROM AspNetUsers U INNER JOIN   AspNetUserRoles UR ON U.Id = UR.UserId INNER JOIN   AspNetRoles R ON UR.RoleId = R.Id WHERE   R.Name IN ({0})";
        var formattedQuery = string.Format(query, string.Join(",", roleNames.Select((_, index) => $"@p{index}")));

        var parameters = roleNames
            .Select((roleName, index) => new SqlParameter($"@p{index}", roleName))
            .ToArray<object>();

        return PagedList<ApplicationUser>.ToPagedList(
            this.Context.Set<ApplicationUser>()
                .FromSqlRaw(formattedQuery, parameters)
                .OrderBy(x => x.UserName), paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }

    public async Task<bool> DoesUserNameExist(string username, CancellationToken token) =>
        await this.Context.Set<ApplicationUser>().AnyAsync(a => a.NormalizedUserName == username.ToUpper(), cancellationToken: token);

    public async Task<bool> DoesEmailExist(string email, CancellationToken token) =>
        await this.Context.Set<ApplicationUser>().AnyAsync(a => a.NormalizedEmail == email.ToUpper(), cancellationToken: token);
}