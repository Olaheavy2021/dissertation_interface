using Microsoft.EntityFrameworkCore;
using Shared.Repository;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.Repository;

public class ApplicationUserRepository: GenericRepository<ApplicationUser>, IApplicationUserRepository
{
    public ApplicationUserRepository(DbContext dbContext) : base(dbContext)
    {

    }
}