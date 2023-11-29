using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;

namespace Dissertation.Application.Course.Commands.CreateCourse;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    private readonly IUnitOfWork _db;

    public CreateCourseCommandValidator(IUnitOfWork db)
    {
        this._db = db;
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Course Name is required");
        RuleFor(x => x)
            .MustAsync(IsCourseNameUnique).WithMessage("Course name must be unique")
            .OverridePropertyName("Name");
        RuleFor(x => x)
            .MustAsync(DoesDepartmentExist).WithMessage("Department does not exist")
            .OverridePropertyName("DepartmentId");
    }

    private async Task<bool> IsCourseNameUnique(CreateCourseCommand request, CancellationToken token) => !await this._db.CourseRepository.AnyAsync(x => x.Name == request.Name);

    private async Task<bool> DoesDepartmentExist(CreateCourseCommand request, CancellationToken token) => await this._db.DepartmentRepository.AnyAsync(x => x.Id == request.DepartmentId);
}