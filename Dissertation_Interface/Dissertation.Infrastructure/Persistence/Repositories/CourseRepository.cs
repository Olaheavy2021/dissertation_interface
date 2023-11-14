using Dissertation.Domain.Entities;
using Dissertation.Infrastructure.Persistence.IRepository;
using Microsoft.EntityFrameworkCore;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.Repositories;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(DbContext dbContext) : base(dbContext)
    {

    }
}