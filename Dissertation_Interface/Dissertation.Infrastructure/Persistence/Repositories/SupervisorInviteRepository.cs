using Dissertation.Domain.Entities;
using Dissertation.Infrastructure.Persistence.IRepository;
using Microsoft.EntityFrameworkCore;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.Repositories;

public class SupervisorInviteRepository : GenericRepository<SupervisorInvite>, ISupervisorInviteRepository
{
    public SupervisorInviteRepository(DbContext dbContext) : base(dbContext)
    {

    }
}