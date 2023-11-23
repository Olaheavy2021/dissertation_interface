using Dissertation.Domain.Entities;
using Dissertation.Infrastructure.Persistence.IRepository;
using Microsoft.EntityFrameworkCore;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.Repositories;

public class SupervisorRepository : GenericRepository<Supervisor>, ISupervisorRepository
{
    public SupervisorRepository(DbContext dbContext) : base(dbContext)
    {

    }
}