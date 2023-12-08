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

public class AcademicYearRepository : GenericRepository<AcademicYear>, IAcademicYearRepository
{
    public AcademicYearRepository(DbContext dbContext) : base(dbContext)
    {

    }

    public async Task<bool> IsAcademicYearUnique(DateTime startDate, DateTime endDate) =>
        !await this.Context.Set<AcademicYear>().AnyAsync(a =>
            a.StartDate.Year == startDate.Year || a.EndDate.Year == endDate.Year);

    public async Task<AcademicYear?> GetActiveAcademicYear() =>
        await this.Context.Set<AcademicYear>().FirstOrDefaultAsync(a =>
            a.StartDate.Date <= DateTime.UtcNow.Date && a.EndDate.Date >= DateTime.UtcNow.Date);

    public PagedList<AcademicYear> GetListOfAcademicYears(
        AcademicYearPaginationParameters paginationParameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT * FROM AcademicYears");

        // Apply search
        if (paginationParameters.SearchByYear > 0)
        {
            sqlQuery.Append(" WHERE YEAR(StartDate) = @search");
            parametersList.Add(new SqlParameter("@search", $"{paginationParameters.SearchByYear}"));
        }

        return PagedList<AcademicYear>.ToPagedList(
            this.Context.Set<AcademicYear>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .OrderByDescending(x => x.CreatedAt), paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }
}