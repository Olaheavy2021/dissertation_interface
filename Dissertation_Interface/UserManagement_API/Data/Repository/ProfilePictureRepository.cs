using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Shared.Repository;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;

namespace UserManagement_API.Data.Repository;

[ExcludeFromCodeCoverage]
public class ProfilePictureRepository : GenericRepository<ProfilePicture>, IProfilePictureRepository
{
    public ProfilePictureRepository(DbContext dbContext) : base(dbContext)
    {
    }
}