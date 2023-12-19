using Microsoft.EntityFrameworkCore;
using Shared.Repository;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Data.Repository;

public class ProfilePictureRepository : GenericRepository<ProfilePicture>, IProfilePictureRepository
{
    public ProfilePictureRepository(DbContext dbContext) : base(dbContext)
    {
    }
}