using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;
using Shared.Enums;
using Shared.Helpers;
using Shared.Repository;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Data.Repository;

public class SupervisionRequestRepository : GenericRepository<SupervisionRequest>, ISupervisionRequestRepository
{
    public SupervisionRequestRepository(DbContext dbContext) : base(dbContext)
    {

    }

    public PagedList<SupervisionRequest> GetPaginatedListOfSupervisionRequests(SupervisionRequestPaginationParameters parameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT SR.*, " +
                                         " U1.Id AS SupervisorIdentifier, " +
                                         " U2.Id AS StudentIdentifier " +
                                         " FROM SupervisionRequests SR" +
                                         " INNER JOIN AspNetUsers U1 ON U1.Id = SR.SupervisorId " +
                                         " INNER JOIN AspNetUsers U2 ON U2.Id = SR.StudentId " +
                                         " WHERE SR.DissertationCohortId = @cohortId ");

        parametersList.Add(new SqlParameter("@cohortId", parameters.DissertationCohortId));

        if (parameters.FilterByStatus != SupervisionRequestStatus.None)
        {
            sqlQuery.Append(" AND SR.Status = @status");
            parametersList.Add(new SqlParameter("@status", parameters.FilterByStatus));
        }

        if (!string.IsNullOrEmpty(parameters.SearchBySupervisor))
        {
            sqlQuery.Append(" AND U1.Lastname LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{parameters.SearchBySupervisor}%"));
        }

        if (!string.IsNullOrEmpty(parameters.SearchByStudent))
        {
            sqlQuery.Append(" AND U2.Lastname LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{parameters.SearchByStudent}%"));
        }

        return PagedList<SupervisionRequest>.ToPagedList(
            this.Context.Set<SupervisionRequest>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .Include(x => x.Supervisor)
                .Include(x => x.Student)
                .Include(x => x.Supervisor.ProfilePicture)
                .Include(x => x.Student.ProfilePicture)
                .OrderByDescending(x => x.CreatedAt), parameters.PageNumber,
            parameters.PageSize);
    }

    public PagedList<SupervisionRequest> GetStudentListOfSupervisionRequests(SupervisionRequestPaginationParameters parameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT SR.*, " +
                                         " U1.Id AS SupervisorIdentifier, " +
                                         " U2.Id AS StudentIdentifier " +
                                         " FROM SupervisionRequests SR" +
                                         " INNER JOIN AspNetUsers U1 ON U1.Id = SR.SupervisorId " +
                                         " INNER JOIN AspNetUsers U2 ON U2.Id = SR.StudentId " +
                                         " WHERE SR.DissertationCohortId = @cohortId ");

        parametersList.Add(new SqlParameter("@cohortId", parameters.DissertationCohortId));

        if (!string.IsNullOrEmpty(parameters.StudentId))
        {
            sqlQuery.Append(" AND SR.StudentId = @studentId");
            parametersList.Add(new SqlParameter("@studentId", parameters.StudentId));
        }

        if (parameters.FilterByStatus != SupervisionRequestStatus.None)
        {
            sqlQuery.Append(" AND SR.Status = @status");
            parametersList.Add(new SqlParameter("@status", parameters.FilterByStatus));
        }

        if (!string.IsNullOrEmpty(parameters.SearchBySupervisor))
        {
            sqlQuery.Append(" AND U1.Lastname LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{parameters.SearchBySupervisor}%"));
        }

        return PagedList<SupervisionRequest>.ToPagedList(
            this.Context.Set<SupervisionRequest>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .Include(x => x.Supervisor)
                .Include(x => x.Student)
                .Include(x => x.Supervisor.ProfilePicture)
                .Include(x => x.Student.ProfilePicture)
                .OrderByDescending(x => x.CreatedAt), parameters.PageNumber,
            parameters.PageSize);
    }

    public PagedList<SupervisionRequest> GetSupervisorListOfSupervisionRequests(SupervisionRequestPaginationParameters parameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT SR.*, " +
                                         " U1.Id AS SupervisorIdentifier, " +
                                         " U2.Id AS StudentIdentifier " +
                                         " FROM SupervisionRequests SR" +
                                         " INNER JOIN AspNetUsers U1 ON U1.Id = SR.SupervisorId " +
                                         " INNER JOIN AspNetUsers U2 ON U2.Id = SR.StudentId " +
                                         " WHERE SR.DissertationCohortId = @cohortId ");

        parametersList.Add(new SqlParameter("@cohortId", parameters.DissertationCohortId));

        if (!string.IsNullOrEmpty(parameters.SupervisorId))
        {
            sqlQuery.Append(" AND SR.SupervisorId = @supervisorId");
            parametersList.Add(new SqlParameter("@supervisorId", parameters.SupervisorId));
        }

        if (parameters.FilterByStatus != SupervisionRequestStatus.None)
        {
            sqlQuery.Append(" AND SR.Status = @status");
            parametersList.Add(new SqlParameter("@status", parameters.FilterByStatus));
        }

        if (!string.IsNullOrEmpty(parameters.SearchByStudent))
        {
            sqlQuery.Append(" AND U2.Lastname LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{parameters.SearchByStudent}%"));
        }

        return PagedList<SupervisionRequest>.ToPagedList(
            this.Context.Set<SupervisionRequest>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .Include(x => x.Supervisor)
                .Include(x => x.Student)
                .Include(x => x.Supervisor.ProfilePicture)
                .Include(x => x.Student.ProfilePicture)
                .OrderByDescending(x => x.CreatedAt), parameters.PageNumber,
            parameters.PageSize);
    }
}