using Dissertation.Application.Course.Commands.CreateCourse;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;

namespace Dissertation.Application.Course.Commands.UpdateCourse;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    private readonly IUnitOfWork _db;

    public UpdateCourseCommandValidator(IUnitOfWork db)
    {
        this._db = db;
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Course Name is required");
        RuleFor(x => x)
            .MustAsync(DoesDepartmentExist).WithMessage("Department does not exist")
            .OverridePropertyName("DepartmentId");
    }
    private async Task<bool> DoesDepartmentExist(UpdateCourseCommand request, CancellationToken token) => await this._db.DepartmentRepository.AnyAsync(x => x.Id == request.DepartmentId);
}