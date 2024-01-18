using Dissertation.Domain.Entities;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.IRepository;

public interface ISupervisorSuggestionRepository : IGenericRepository<SupervisorSuggestion>
{
    Task DeleteAllRecords();
    Task<bool> IsSupervisorSuggestionTableEmptyAsync();
}