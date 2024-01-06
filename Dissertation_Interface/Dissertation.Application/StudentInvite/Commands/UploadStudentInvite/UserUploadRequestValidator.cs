using Dissertation.Infrastructure.DTO;
using FluentValidation;

namespace Dissertation.Application.StudentInvite.Commands.UploadStudentInvite;

public class UserUploadRequestValidator : AbstractValidator<UserUploadRequest>
{
    public UserUploadRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty()
            .WithMessage("FirstName is required for each Invite.");

        RuleFor(request => request.LastName)
            .NotEmpty()
            .WithMessage("LastName is required for each Invite.");

        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("Email is required for each Invite.");

        RuleFor(request => request.Username)
            .NotEmpty()
            .WithMessage("Username is required for each Invite.");
    }
}