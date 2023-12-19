using Dissertation.Domain.Entities;
using Dissertation.Infrastructure.Persistence.IRepository;
using Microsoft.EntityFrameworkCore;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.Repositories;

public class ResearchProposalRepository : GenericRepository<ResearchProposal>, IResearchProposalRepository
{
    public ResearchProposalRepository(DbContext dbContext) : base(dbContext)
    {
    }
}