using System.Text;
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

    public PagedList<ApplicationUser> GetPaginatedAdminUsers(UserPaginationParameters paginationParameters)
    {
        string[] roleNames = { "SuperAdmin", "Admin" };
        var sqlQuery = new StringBuilder("SELECT U.*, R.Name " +
                                         "FROM AspNetUsers U " +
                                         "INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId " +
                                         "INNER JOIN AspNetRoles R ON UR.RoleId = R.Id " +
                                         "WHERE R.Name IN ({0}) AND U.Id != @currentUserId");

        // Parameters for query including roles and currentUserId.
        var parametersList = roleNames
            .Select((roleName, index) => new SqlParameter($"@p{index}", roleName))
            .Concat(new[] { new SqlParameter("@currentUserId", paginationParameters.LoggedInAdminId) }).ToList();

        if (!string.IsNullOrEmpty(paginationParameters.SearchByUserName))
        {
            sqlQuery.Append(" AND U.UserName LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{paginationParameters.SearchByUserName}%"));
        }

        // FormattedQuery prepares the placeholders for role names.
        var formattedQuery = string.Format(sqlQuery.ToString(), string.Join(",", roleNames.Select((_, index) => $"@p{index}")));

        return PagedList<ApplicationUser>.ToPagedList(
            this.Context.Set<ApplicationUser>()
                .FromSqlRaw(formattedQuery, parametersList.ToArray<object>())
                .OrderBy(x => x.CreatedOn), paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }

    public async Task<bool> DoesUserNameExist(string username, CancellationToken token) =>
        await this.Context.Set<ApplicationUser>().AnyAsync(a => a.NormalizedUserName == username.ToUpper(), cancellationToken: token);

    public async Task<bool> DoesEmailExist(string? email, CancellationToken token) =>
        await this.Context.Set<ApplicationUser>().AnyAsync(a => a.NormalizedEmail == email.ToUpper(), cancellationToken: token);
}