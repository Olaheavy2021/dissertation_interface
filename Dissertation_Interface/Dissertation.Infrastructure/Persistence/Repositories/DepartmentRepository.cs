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

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(DbContext dbContext) : base(dbContext)
    {

    }

    public PagedList<Department> GetListOfDepartments(DepartmentPaginationParameters paginationParameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT * FROM Departments");

        // Apply search
        if (!string.IsNullOrEmpty(paginationParameters.SearchByName))
        {
            sqlQuery.Append(" WHERE Name LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{paginationParameters.SearchByName}%"));
        }

        // Apply filter
        if (!string.IsNullOrEmpty(paginationParameters.FilterByStatus) && Enum.IsDefined(typeof(DissertationConfigStatus), paginationParameters.FilterByStatus))
        {
            var status = (DissertationConfigStatus)Enum.Parse(typeof(DissertationConfigStatus), paginationParameters.FilterByStatus);
            var whereOrAnd = sqlQuery.ToString().Contains("WHERE") ? "AND" : "WHERE";
            sqlQuery.Append(" {whereOrAnd} Status = @filter");
            parametersList.Add(new SqlParameter("@filter", status));
        }

        return PagedList<Department>.ToPagedList(
            this.Context.Set<Department>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .OrderBy(x => x.Name), paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }
}