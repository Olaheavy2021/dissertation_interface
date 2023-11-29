namespace Dissertation.Infrastructure.Persistence.IRepository;

public interface IUnitOfWork
{
    IAcademicYearRepository AcademicYearRepository { get; }

    IDissertationCohortRepository DissertationCohortRepository { get; }

    IDepartmentRepository DepartmentRepository { get; }

    ICourseRepository CourseRepository { get; }

    ISupervisorInviteRepository SupervisorInviteRepository { get; }

    ISupervisorRepository SupervisorRepository { get; }

    IStudentRepository StudentRepository { get; }

    IStudentInviteRepository StudentInviteRepository { get; }

    Task BeginTransactionAsync();
    Task<int> SaveAsync();
    Task<int> SaveAsync(CancellationToken cancellationToken);
    Task CommitAsync();
    Task RollbackAsync();
}