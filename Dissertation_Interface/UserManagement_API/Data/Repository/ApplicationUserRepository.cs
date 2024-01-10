using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;
using Shared.Helpers;
using Shared.Repository;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.Repository;

[ExcludeFromCodeCoverage]
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
            sqlQuery.Append(" AND U.LastName LIKE @search OR U.FirstName LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{paginationParameters.SearchByUserName}%"));
        }

        // FormattedQuery prepares the placeholders for role names.
        var formattedQuery = string.Format(sqlQuery.ToString(), string.Join(",", roleNames.Select((_, index) => $"@p{index}")));

        return PagedList<ApplicationUser>.ToPagedList(
            this.Context.Set<ApplicationUser>()
                .FromSqlRaw(formattedQuery, parametersList.ToArray<object>())
                .Include(x => x.ProfilePicture)
                .OrderByDescending(x => x.CreatedOn), paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }

    public PagedList<ApplicationUser> GetPaginatedStudents(DissertationStudentPaginationParameters paginationParameters)
    {
        string[] roleNames = { "Student" };
        var sqlQuery = new StringBuilder("SELECT U.*, R.Name " +
                                         "FROM AspNetUsers U " +
                                         "INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId " +
                                         "INNER JOIN AspNetRoles R ON UR.RoleId = R.Id " +
                                         "WHERE R.Name IN ({0}) AND U.CreatedOn BETWEEN @cohortStartDate AND @cohortEndDate ");

        // Parameters for query including roles and currentUserId.
        var parametersList = roleNames
            .Select((roleName, index) => new SqlParameter($"@p{index}", roleName))
            .Concat(new[] { new SqlParameter("@cohortStartDate", paginationParameters.CohortStartDate),
                new SqlParameter("@cohortEndDate", paginationParameters.CohortEndDate) }).ToList();

        if (!string.IsNullOrEmpty(paginationParameters.SearchByUserName))
        {
            sqlQuery.Append(" AND U.LastName LIKE @search OR U.FirstName LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{paginationParameters.SearchByUserName}%"));
        }

        if (paginationParameters.FilterByCourse > 0)
        {
            sqlQuery.Append(" AND U.CourseId = @filter");
            parametersList.Add(new SqlParameter("@filter", $"{paginationParameters.FilterByCourse}"));
        }

        // FormattedQuery prepares the placeholders for role names.
        var formattedQuery = string.Format(sqlQuery.ToString(), string.Join(",", roleNames.Select((_, index) => $"@p{index}")));

        return PagedList<ApplicationUser>.ToPagedList(
            this.Context.Set<ApplicationUser>()
                .FromSqlRaw(formattedQuery, parametersList.ToArray<object>())
                .Include(x => x.ProfilePicture)
                .OrderByDescending(x => x.CreatedOn), paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }

    public PagedList<ApplicationUser> GetPaginatedSupervisors(SupervisorPaginationParameters paginationParameters)
    {
        string[] roleNames = { "Supervisor" };
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
            sqlQuery.Append(" AND U.LastName LIKE @search OR U.FirstName LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{paginationParameters.SearchByUserName}%"));
        }

        if (paginationParameters.FilterByDepartment > 0)
        {
            sqlQuery.Append(" AND U.CourseId = @filter");
            parametersList.Add(new SqlParameter("@filter", $"{paginationParameters.FilterByDepartment}"));
        }

        // FormattedQuery prepares the placeholders for role names.
        var formattedQuery = string.Format(sqlQuery.ToString(), string.Join(",", roleNames.Select((_, index) => $"@p{index}")));

        return PagedList<ApplicationUser>.ToPagedList(
            this.Context.Set<ApplicationUser>()
                .FromSqlRaw(formattedQuery, parametersList.ToArray<object>())
                .Include(x => x.ProfilePicture)
                .OrderByDescending(x => x.CreatedOn), paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }

    public async Task<bool> DoesUserNameExist(string username, CancellationToken token) =>
        await this.Context.Set<ApplicationUser>().AnyAsync(a => a.NormalizedUserName == username.ToUpper(), cancellationToken: token);

    public async Task<bool> DoesEmailExist(string? email, CancellationToken token) =>
        await this.Context.Set<ApplicationUser>().AnyAsync(a => a.NormalizedEmail == email.ToUpper(), cancellationToken: token);
}