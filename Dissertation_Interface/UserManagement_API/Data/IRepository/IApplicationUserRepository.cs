using Shared.Repository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.IRepository;

public interface IApplicationUserRepository : IGenericRepository<ApplicationUser>
{
}