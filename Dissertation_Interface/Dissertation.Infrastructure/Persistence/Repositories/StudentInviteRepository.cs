using System.Text;
using Dissertation.Domain.Entities;
using Dissertation.Domain.Pagination;
using Dissertation.Infrastructure.Persistence.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.Repositories;

public class StudentInviteRepository : GenericRepository<StudentInvite>, IStudentInviteRepository
{
    public StudentInviteRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public PagedList<StudentInvite> GetListOfStudentInvites(StudentInvitePaginationParameters paginationParameters,
        long cohortId)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT * FROM StudentInvites");

        // Apply search
        if (!string.IsNullOrEmpty(paginationParameters.SearchByStudentId))
        {
            sqlQuery.Append(" WHERE Name LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{paginationParameters.SearchByStudentId}%"));
        }

        if (paginationParameters.FilterByCohortId > 0)
        {
            var whereOrAnd = sqlQuery.ToString().Contains("WHERE") ? "AND" : "WHERE";
            sqlQuery.Append($" {whereOrAnd} DissertationCohortId = @filter");
            parametersList.Add(new SqlParameter("@filter", paginationParameters.FilterByCohortId));
        }
        else
        {
            var whereOrAnd = sqlQuery.ToString().Contains("WHERE") ? "AND" : "WHERE";
            sqlQuery.Append($" {whereOrAnd} DissertationCohortId = @filter");
            parametersList.Add(new SqlParameter("@filter", cohortId));
        }

        if (paginationParameters.FilterByCourseId > 0)
        {
            var whereOrAnd = sqlQuery.ToString().Contains("WHERE") ? "AND" : "WHERE";
            sqlQuery.Append($" {whereOrAnd} CourseId = @filter");
            parametersList.Add(new SqlParameter("@filter", paginationParameters.FilterByCourseId));
        }

        return PagedList<StudentInvite>.ToPagedList(
            this.Context.Set<StudentInvite>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .OrderByDescending(x => x.CreatedAt), paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }

}