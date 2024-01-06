using FluentValidation;
using Shared.DTO;

namespace UserManagement_API.Data.Models.Validators;

public class CreateSupervisionCohortListRequestValidator : AbstractValidator<CreateSupervisionCohortListRequest>
{
    public CreateSupervisionCohortListRequestValidator()
    {

    }
}