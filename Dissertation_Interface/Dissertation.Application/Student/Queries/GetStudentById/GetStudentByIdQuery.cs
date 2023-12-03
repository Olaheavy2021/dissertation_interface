using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Student.Queries.GetStudentById;

public sealed record GetStudentByIdQuery(string Id) : IRequest<ResponseDto<GetStudent>>;