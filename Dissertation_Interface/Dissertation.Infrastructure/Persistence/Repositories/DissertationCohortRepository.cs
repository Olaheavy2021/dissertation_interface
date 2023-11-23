﻿using System.Text;
using Dissertation.Domain.Entities;
using Dissertation.Domain.Enums;
using Dissertation.Domain.Pagination;
using Dissertation.Infrastructure.Persistence.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.Repositories;

public class DissertationCohortRepository: GenericRepository<DissertationCohort>, IDissertationCohortRepository
{
    public DissertationCohortRepository(DbContext dbContext) : base(dbContext)
    {

    }

    public PagedList<DissertationCohort> GetListOfDissertationCohort(DissertationCohortPaginationParameters paginationParameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT * FROM DissertationCohorts");

        // Apply search
        if (paginationParameters.SearchByStartYear > 0)
        {
            sqlQuery.Append(" WHERE YEAR(StartDate) = @search");
            parametersList.Add(new SqlParameter("@search", $"%{paginationParameters.SearchByStartYear}%"));
        }

        // Apply filter
        if (!string.IsNullOrEmpty(paginationParameters.FilterByStatus) && Enum.IsDefined(typeof(DissertationConfigStatus), paginationParameters.FilterByStatus))
        {
            var status = (DissertationConfigStatus)Enum.Parse(typeof(DissertationConfigStatus), paginationParameters.FilterByStatus);
            var whereOrAnd = sqlQuery.ToString().Contains("WHERE") ? "AND" : "WHERE";
            sqlQuery.Append($" {whereOrAnd} Status = @filter");
            parametersList.Add(new SqlParameter("@filter", status));
        }

        return PagedList<DissertationCohort>.ToPagedList(
            this.Context.Set<DissertationCohort>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .Include(x => x.AcademicYear)
                .OrderBy(x => x.CreatedAt), paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }

    public async Task<DissertationCohort?> GetActiveDissertationCohort(long id) => await this.Context.Set<DissertationCohort>().FirstOrDefaultAsync(a => a.Status == DissertationConfigStatus.Active);
}