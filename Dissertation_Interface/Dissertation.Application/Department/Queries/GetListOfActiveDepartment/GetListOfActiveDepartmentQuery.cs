using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Department.Queries.GetListOfActiveDepartment;

public sealed record GetListOfActiveDepartment() : IRequest<ResponseDto<List<GetDepartment>>>;