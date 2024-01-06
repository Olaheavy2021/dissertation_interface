using FluentValidation;

namespace Dissertation.Application.StudentInvite.Commands.UploadStudentInvite;

public class UploadStudentInviteCommandValidator : AbstractValidator<UploadStudentInviteCommand>
{
    public UploadStudentInviteCommandValidator() => RuleForEach(x => x.Requests).SetValidator(new UserUploadRequestValidator());
}
