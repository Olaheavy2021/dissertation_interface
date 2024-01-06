using Dissertation.Application.StudentInvite.Commands.UploadStudentInvite;
using FluentValidation;

namespace Dissertation.Application.SupervisorInvite.Commands.UploadSupervisorInvite;

public class UploadSupervisorInviteCommandValidator : AbstractValidator<UploadSupervisorInviteCommand>
{
    public UploadSupervisorInviteCommandValidator() => RuleForEach(x => x.Requests).SetValidator(new UserUploadRequestValidator());
}