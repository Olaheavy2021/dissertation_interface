using Dissertation.Domain.Entities;
using Dissertation.Infrastructure.Persistence.IRepository;
using Microsoft.EntityFrameworkCore;
using Shared.Repository;

namespace Dissertation.Infrastructure.Persistence.Repositories;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(DbContext dbContext) : base(dbContext)
    {
    }
}