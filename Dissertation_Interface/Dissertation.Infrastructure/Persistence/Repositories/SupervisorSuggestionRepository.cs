using Dissertation.Domain.Entities;
using Dissertation.Infrastructure.Persistence.IRepository;
using Microsoft.EntityFrameworkCore;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.Repositories;

public class SupervisorSuggestionRepository : GenericRepository<SupervisorSuggestion>, ISupervisorSuggestionRepository
{
    public SupervisorSuggestionRepository(DbContext dbContext) : base(dbContext)
    {

    }

    public async Task DeleteAllRecords() => await this.Context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE SupervisorSuggestions");

    public async Task<bool> IsSupervisorSuggestionTableEmptyAsync() =>
        // Check if any records exist in the SupervisorSuggestion table
        !await this.Context.Set<SupervisorSuggestion>().AnyAsync();
}