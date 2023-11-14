using System.Text;
using Dissertation.Domain.Entities;
using Dissertation.Domain.Enums;
using Dissertation.Domain.Pagination;
using Dissertation.Infrastructure.Persistence.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;
using Shared.Helpers;
using Shared.Logging;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.Repositories;

public class AcademicYearRepository : GenericRepository<AcademicYear>, IAcademicYearRepository
{
    public AcademicYearRepository(DbContext dbContext) : base(dbContext)
    {

    }

    public async Task<bool> IsAcademicYearUnique(DateTime startDate, DateTime endDate) =>
        !await this.Context.Set<AcademicYear>().AnyAsync(a =>
            a.StartDate.Year == startDate.Year || a.EndDate.Year == endDate.Year);

    public async Task<AcademicYear?> GetActiveAcademicYear() => await this.Context.Set<AcademicYear>().FirstOrDefaultAsync(a => a.Status == DissertationConfigStatus.Active);

    public PagedList<AcademicYear> GetListOfAcademicYears(
        AcademicYearPaginationParameters paginationParameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT * FROM AcademicYears");

        // Apply search
        if (paginationParameters.SearchByYear > 0)
        {
            sqlQuery.Append(" WHERE YEAR(StartDate) = @search");
            parametersList.Add(new SqlParameter("@search", $"%{paginationParameters.SearchByYear}%"));
        }

        // Apply filter
        if (!string.IsNullOrEmpty(paginationParameters.FilterByStatus) && Enum.IsDefined(typeof(DissertationConfigStatus), paginationParameters.FilterByStatus))
        {
            var status = (DissertationConfigStatus)Enum.Parse(typeof(DissertationConfigStatus), paginationParameters.FilterByStatus);
            var whereOrAnd = sqlQuery.ToString().Contains("WHERE") ? "AND" : "WHERE";
            sqlQuery.Append(" {whereOrAnd} Status = @filter");
            parametersList.Add(new SqlParameter("@filter", status));
        }

        return PagedList<AcademicYear>.ToPagedList(
            this.Context.Set<AcademicYear>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .OrderBy(x => x.StartDate), paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }
}