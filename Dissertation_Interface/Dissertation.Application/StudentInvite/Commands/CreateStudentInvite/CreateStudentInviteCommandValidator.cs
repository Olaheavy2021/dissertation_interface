using Dissertation.Domain.Enums;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using FluentValidation;
using Shared.Constants;
using Shared.DTO;
using Shared.Helpers;

namespace Dissertation.Application.SupervisorInvite.Commands.CreateSupervisorInvite;

public class CreateSupervisorInviteCommandValidator : AbstractValidator<CreateSupervisorInviteCommand>
{
    private readonly IUnitOfWork _db;
    private readonly IUserApiService _userApiService;

    public CreateSupervisorInviteCommandValidator(IUnitOfWork db, IUserApiService userApiService)
    {
        this._db = db;
        this._userApiService = userApiService;

        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.StaffId)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(50).WithMessage(ErrorMessages.MaximumLength50)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace);

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MaximumLength(100).WithMessage(ErrorMessages.MaximumLength100)
            .Matches(@"^[a-zA-Z0-9._%+-]+@(student\.shu\.ac\.uk|shu\.ac\.uk|hallam\.shu\.ac\.uk)$").WithMessage(ErrorMessages.MustBeHallamEmailFormat)
            .Matches(@"^\S+$").WithMessage(ErrorMessages.MustNotContainWhiteSpace)
            .EmailAddress();

        RuleFor(q => q)
            .MustAsync(DoesEmailExistsAsStudentOrSupervisor)
            .WithMessage("This email already exists for a Student or Supervisor")
            .OverridePropertyName("Email");

        RuleFor(q => q)
            .MustAsync(DoesUserNameExistsAsStudentOrSupervisor)
            .WithMessage("This username already exists for a Student or Supervisor")
            .OverridePropertyName("StaffId");

        RuleFor(q => q)
            .MustAsync(DoesRequestHaveActiveInvite)
            .WithMessage("The supervisor has an active invite for either the email or the staff id")
            .OverridePropertyName("StaffId");
    }

    private async Task<bool> DoesEmailExistsAsStudentOrSupervisor(CreateSupervisorInviteCommand request, CancellationToken token)
    {
       ResponseDto<GetUserDto> response = await this._userApiService.GetUserByEmail(request.Email);
       if (!response.IsSuccess) return true;
       var isStudentOrSupervisor = response.Result!.Role.Any(role => role.Equals(Roles.RoleStudent, StringComparison.OrdinalIgnoreCase))
                                   || response.Result!.Role.Any(role =>
                                       role.Equals(Roles.RoleSupervisor, StringComparison.OrdinalIgnoreCase));
       return !isStudentOrSupervisor;
    }

    private async Task<bool> DoesUserNameExistsAsStudentOrSupervisor(CreateSupervisorInviteCommand request, CancellationToken token)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByUserName(request.StaffId);
        if (!response.IsSuccess) return true;

        var isStudentOrSupervisor = response.Result!.Role.Any(role => role.Equals(Roles.RoleStudent, StringComparison.OrdinalIgnoreCase))
                                    || response.Result!.Role.Any(role =>
                                        role.Equals(Roles.RoleSupervisor, StringComparison.OrdinalIgnoreCase));
        return !isStudentOrSupervisor;
    }

    private async Task<bool> DoesRequestHaveActiveInvite(CreateSupervisorInviteCommand request, CancellationToken token)
    {
        Domain.Entities.SupervisorInvite? supervisorInvite = await this._db.SupervisorInviteRepository.GetFirstOrDefaultAsync(x =>(
            x.StaffId == request.StaffId.ToLower() || x.Email == request.Email.ToLower()) && x.ExpiryDate.Date > DateTime.UtcNow.Date);

        return supervisorInvite == null;
    }
}