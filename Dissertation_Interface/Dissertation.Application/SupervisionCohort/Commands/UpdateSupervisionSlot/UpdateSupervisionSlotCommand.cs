using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisionCohort.Commands.UpdateSupervisionSlot;

public sealed record UpdateSupervisionSlotCommand(
    int SupervisionSlots,
    long SupervisionCohortId
    ) : IRequest<ResponseDto<string>>;