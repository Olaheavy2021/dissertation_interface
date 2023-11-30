using System.Text;
using Dissertation.Domain.Entities;
using Dissertation.Domain.Pagination;
using Dissertation.Infrastructure.Persistence.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.Repositories;

public class SupervisorInviteRepository : GenericRepository<SupervisorInvite>, ISupervisorInviteRepository
{
    public SupervisorInviteRepository(DbContext dbContext) : base(dbContext)
    {

    }

    public PagedList<SupervisorInvite> GetListOfSupervisorInvites(SupervisorInvitePaginationParameters paginationParameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT * FROM SupervisorInvites");

        // Apply search
        if (!string.IsNullOrEmpty(paginationParameters.SearchByStaffId))
        {
            sqlQuery.Append(" WHERE StaffId LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{paginationParameters.SearchByStaffId}%"));
        }

        return PagedList<SupervisorInvite>.ToPagedList(
            this.Context.Set<SupervisorInvite>()
                .FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>())
                .OrderByDescending(x => x.CreatedAt), paginationParameters.PageNumber,
            paginationParameters.PageSize);
    }
}