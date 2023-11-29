using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.StudentInvite.Queries.GetById;

public sealed record GetStudentInviteByIdQuery(long Id) : IRequest<ResponseDto<GetStudentInvite>>;