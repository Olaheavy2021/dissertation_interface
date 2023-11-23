using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;
using Shared.Constants;
using Shared.DTO;

namespace Dissertation.Application.SupervisorInvite.Commands.UpdateSupervisorInvite;

public class UpdateSupervisorInviteCommandValidator: AbstractValidator<UpdateSupervisorInviteCommand>
{
    private readonly IUnitOfWork _db;
    private readonly IUserApiService _userApiService;

    public UpdateSupervisorInviteCommandValidator(IUnitOfWork db, IUserApiService userApiService)
    {
         this._db = db;
        this._userApiService = userApiService;

        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.StaffId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(100).WithMessage(ErrorMessages.MaximumLength100)
            .Matches(@"^[a-zA-Z0-9._%+-]+@(student\.shu\.ac\.uk|shu\.ac\.uk|hallam\.shu\.ac\.uk)$").WithMessage(ErrorMessages.MustBeHallamEmailFormat)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace)
            .EmailAddress();

        RuleFor(p => p.Department)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .NotNull().WithMessage(ErrorMessages.RequiredField);

        RuleFor(q => q)
            .MustAsync(DoesEmailExistsAsStudentOrSupervisor)
            .WithMessage("This email already exists for a Student or Supervisor")
            .OverridePropertyName("Email");

        RuleFor(q => q)
            .MustAsync(DoesUserNameExistsAsStudentOrSupervisor)
            .WithMessage("This username already exists for a Student or Supervisor")
            .OverridePropertyName("StaffId");
        RuleFor(q => q)
            .MustAsync(DoesUserNameExistsAsStudentOrSupervisor)
            .WithMessage("This username already exists for a Student or Supervisor")
            .OverridePropertyName("StaffId");
        RuleFor(q => q)
            .MustAsync(IsSupervisionInviteActive)
            .WithMessage("Supervision Invite is not active")
            .OverridePropertyName("Id");
    }

    private async Task<bool> DoesEmailExistsAsStudentOrSupervisor(UpdateSupervisorInviteCommand request, CancellationToken token)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByEmail(request.Email);
        if (!response.IsSuccess) return true;
        if (request.Email == response.Result!.User?.Email) return true;

        var isAStudent =  response.Result!.Role.Contains(Roles.RoleStudent);
        var isASupervisor =  response.Result!.Role.Contains(Roles.RoleSupervisor);
        return !isAStudent && !isASupervisor;
    }

    private async Task<bool> DoesUserNameExistsAsStudentOrSupervisor(UpdateSupervisorInviteCommand request, CancellationToken token)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByUserName(request.StaffId);
        if (!response.IsSuccess) return true;
        if (request.Email == response.Result!.User?.Email) return true;

        var isAStudent =  response.Result!.Role.Contains(Roles.RoleStudent);
        var isASupervisor =  response.Result!.Role.Contains(Roles.RoleSupervisor);
        return !isAStudent && !isASupervisor;
    }

    private async Task<bool> IsSupervisionInviteActive(UpdateSupervisorInviteCommand request, CancellationToken token)
    {
        Domain.Entities.SupervisorInvite? supervisionInvite = await this._db.SupervisorInviteRepository.GetFirstOrDefaultAsync(x => x.Id == request.Id);
        return supervisionInvite != null && supervisionInvite.ExpiryDate.Date > DateTime.UtcNow.Date;
    }
}