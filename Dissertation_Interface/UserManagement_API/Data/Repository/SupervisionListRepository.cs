using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;
using Shared.Helpers;
using Shared.Repository;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Data.Repository;

public class SupervisionListRepository : GenericRepository<SupervisionList>, ISupervisionListRepository
{
    public SupervisionListRepository(DbContext dbContext) : base(dbContext)
    {

    }

    public PagedList<SupervisionList> GetSupervisionListsForSupervisor(
        SupervisionListPaginationParameters parameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT SL.*, " +
                                         " U1.Id AS SupervisorIdentifier," +
                                         " U2.Id AS StudentIdentifier " +
                                         " FROM SupervisionLists SL " +
                                         " INNER JOIN AspNetUsers U1 ON U1.Id = SL.SupervisorId " +
                                         " INNER JOIN AspNetUsers U2 ON U2.Id = SL.StudentId " +
                                         " WHERE SL.DissertationCohortId = @cohortId ");

        parametersList.Add(new SqlParameter("@cohortId", parameters.DissertationCohortId));

        if (!string.IsNullOrEmpty(parameters.SupervisorId))
        {
            sqlQuery.Append(" AND SL.SupervisorId = @supervisorId");
            parametersList.Add(new SqlParameter("@supervisorId", parameters.SupervisorId));
        }

        if (!string.IsNullOrEmpty(parameters.SearchByStudent))
        {
            sqlQuery.Append(" AND U2.Lastname LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{parameters.SearchByStudent}%"));
        }

        return PagedList<SupervisionList>.ToPagedList(
            this.Context.Set<SupervisionList>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .Include(x => x.Supervisor)
                .Include(x => x.Student)
                .OrderByDescending(x => x.CreatedAt), parameters.PageNumber,
            parameters.PageSize);
    }

    public PagedList<SupervisionList> GetSupervisionListsForStudent(
        SupervisionListPaginationParameters parameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT SL.*, " +
                                         " U1.Id AS SupervisorIdentifier," +
                                         " U2.Id AS StudentIdentifier " +
                                         " FROM SupervisionLists SL " +
                                         " INNER JOIN AspNetUsers U1 ON U1.Id = SL.SupervisorId " +
                                         " INNER JOIN AspNetUsers U2 ON U2.Id = SL.StudentId " +
                                         " WHERE SL.DissertationCohortId = @cohortId ");

        parametersList.Add(new SqlParameter("@cohortId", parameters.DissertationCohortId));

        if (!string.IsNullOrEmpty(parameters.StudentId))
        {
            sqlQuery.Append(" AND SL.StudentId = @studentId");
            parametersList.Add(new SqlParameter("@studentId", parameters.StudentId));
        }

        if (!string.IsNullOrEmpty(parameters.SearchBySupervisor))
        {
            sqlQuery.Append(" AND U1.Lastname LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{parameters.SearchBySupervisor}%"));
        }

        return PagedList<SupervisionList>.ToPagedList(
            this.Context.Set<SupervisionList>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .Include(x => x.Supervisor)
                .Include(x => x.Student)
                .OrderByDescending(x => x.CreatedAt), parameters.PageNumber,
            parameters.PageSize);
    }

    public PagedList<SupervisionList> GetPaginatedListOfSupervisionLists(
        SupervisionListPaginationParameters parameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT SL.*, " +
                                         " U1.Id AS SupervisorIdentifier," +
                                         " U2.Id AS StudentIdentifier " +
                                         " FROM SupervisionLists SL " +
                                         " INNER JOIN AspNetUsers U1 ON U1.Id = SL.SupervisorId " +
                                         " INNER JOIN AspNetUsers U2 ON U2.Id = SL.StudentId " +
                                         " WHERE SL.DissertationCohortId = @cohortId ");

        parametersList.Add(new SqlParameter("@cohortId", parameters.DissertationCohortId));

        if (!string.IsNullOrEmpty(parameters.SearchBySupervisor))
        {
            sqlQuery.Append(" AND U1.LastName LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{parameters.SearchBySupervisor}%"));
        }

        return PagedList<SupervisionList>.ToPagedList(
            this.Context.Set<SupervisionList>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .Include(x => x.Supervisor)
                .Include(x => x.Student)
                .OrderByDescending(x => x.CreatedAt), parameters.PageNumber,
            parameters.PageSize);
    }
}