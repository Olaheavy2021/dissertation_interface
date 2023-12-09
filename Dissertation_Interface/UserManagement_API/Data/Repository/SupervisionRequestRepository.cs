using Microsoft.EntityFrameworkCore;
using Shared.Repository;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Data.Repository;

public class SupervisionRequestRepository : GenericRepository<SupervisionRequest>, ISupervisionRequestRepository
{
    public SupervisionRequestRepository(DbContext dbContext) : base(dbContext)
    {

    }

}