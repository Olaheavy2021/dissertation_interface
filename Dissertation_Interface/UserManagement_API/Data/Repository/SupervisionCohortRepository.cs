using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;
using Shared.Helpers;
using Shared.Repository;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Data.Repository;

public class SupervisionCohortRepository : GenericRepository<SupervisionCohort>, ISupervisionCohortRepository
{
    public SupervisionCohortRepository(DbContext dbContext) : base(dbContext)
    {

    }

    public PagedList<ApplicationUser> GetActiveSupervisors(SupervisionCohortListParameters listParameters)
    {
        string[] roleNames = { "Supervisor" };
        var sqlQuery = new StringBuilder("SELECT U.*, R.Name " +
                                         " FROM AspNetUsers U " +
                                         " INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId " +
                                         " INNER JOIN AspNetRoles R ON UR.RoleId = R.Id " +
                                         " INNER JOIN SupervisionCohorts S ON U.Id = S.SupervisorId " +
                                         " WHERE R.Name IN ({0}) AND " +
                                         " AND S.DissertationCohortId = @cohortId");
        // Parameters for query including roles and currentUserId.
        var parametersList = roleNames
            .Select((roleName, index) => new SqlParameter($"@p{index}", roleName))
            .Concat(new[] { new SqlParameter("@cohortId", listParameters.DissertationCohortId) }).ToList();

        if (!string.IsNullOrEmpty(listParameters.SearchByUserName))
        {
            sqlQuery.Append(" AND U.LastName LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{listParameters.SearchByUserName}%"));
        }

        // FormattedQuery prepares the placeholders for role names.
        var formattedQuery = string.Format(sqlQuery.ToString(), string.Join(",", roleNames.Select((_, index) => $"@p{index}")));

        return PagedList<ApplicationUser>.ToPagedList(
            this.Context.Set<ApplicationUser>()
                .FromSqlRaw(formattedQuery, parametersList.ToArray<object>())
                .Include(x => x.ProfilePicture)
                .OrderByDescending(x => x.CreatedOn), listParameters.PageNumber,
            listParameters.PageSize);
    }

    public PagedList<ApplicationUser> GetInActiveSupervisors(SupervisionCohortListParameters listParameters)
    {
        string[] roleNames = { "Supervisor" };
        var sqlQuery = new StringBuilder("SELECT U.*, R.Name " +
                                         " FROM AspNetUsers U " +
                                         " INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId " +
                                         " INNER JOIN AspNetRoles R ON UR.RoleId = R.Id " +
                                         " WHERE R.Name IN ({0}) AND NOT EXISTS " +
                                         " ( SELECT 1 FROM SupervisionCohorts WHERE SupervisionCohorts.SupervisorId = U.Id " +
                                         " AND SupervisionCohorts.DissertationCohortId = @cohortId)");

        // Parameters for query including roles and currentUserId.
        var parametersList = roleNames
            .Select((roleName, index) => new SqlParameter($"@p{index}", roleName))
            .Concat(new[] { new SqlParameter("@cohortId", listParameters.DissertationCohortId) }).ToList();

        if (!string.IsNullOrEmpty(listParameters.SearchByUserName))
        {
            sqlQuery.Append(" AND U.LastName LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{listParameters.SearchByUserName}%"));
        }

        // FormattedQuery prepares the placeholders for role names.
        var formattedQuery = string.Format(sqlQuery.ToString(), string.Join(",", roleNames.Select((_, index) => $"@p{index}")));

        return PagedList<ApplicationUser>.ToPagedList(
            this.Context.Set<ApplicationUser>()
                .FromSqlRaw(formattedQuery, parametersList.ToArray<object>())
                .Include(x => x.ProfilePicture)
                .OrderByDescending(x => x.CreatedOn), listParameters.PageNumber,
            listParameters.PageSize);
    }

    public PagedList<SupervisionCohort> GetPaginatedListOfSupervisionCohort(SupervisionCohortListParameters listParameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT S.*, U.Email " +
                                         " FROM SupervisionCohorts S" +
                                         " INNER JOIN AspNetUsers U ON S.SupervisorId = U.Id " +
                                         " WHERE S.DissertationCohortId = @cohortId");

        parametersList.Add(new SqlParameter("@cohortId", listParameters.DissertationCohortId));

        if (!string.IsNullOrEmpty(listParameters.SearchByUserName))
        {
            sqlQuery.Append(" AND U.LastName LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{listParameters.SearchByUserName}%"));
        }

        return PagedList<SupervisionCohort>.ToPagedList(
            this.Context.Set<SupervisionCohort>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .Include(x => x.Supervisor)
                .Include(x => x.Supervisor.ProfilePicture)
                .OrderByDescending(x => x.CreatedAt), listParameters.PageNumber,
            listParameters.PageSize);
    }
}