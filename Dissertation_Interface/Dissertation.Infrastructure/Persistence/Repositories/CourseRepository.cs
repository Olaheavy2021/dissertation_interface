using System.Text;
using Dissertation.Domain.Entities;
using Dissertation.Domain.Enums;
using Dissertation.Domain.Pagination;
using Dissertation.Infrastructure.Persistence.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.Repositories;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(DbContext dbContext) : base(dbContext)
    {

    }

    public PagedList<Course> GetListOfCourse(CoursePaginationParameters paginationParameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT * FROM Courses");

        // Apply search
        if (!string.IsNullOrEmpty(paginationParameters.SearchByName))
        {
            sqlQuery.Append(" WHERE Name LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{paginationParameters.SearchByName}%"));
        }

        // Apply filter
        if (!string.IsNullOrEmpty(paginationParameters.FilterByDepartment))
        {
            var whereOrAnd = sqlQuery.ToString().Contains("WHERE") ? "AND" : "WHERE";
            sqlQuery.Append($" {whereOrAnd} DepartmentId = @filter");
            parametersList.Add(new SqlParameter("@filter", paginationParameters.FilterByDepartment));
        }

        return PagedList<Course>.ToPagedList(
            this.Context.Set<Course>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .Include(c => c.Department)
                .OrderByDescending(x => x.CreatedAt), paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }
}