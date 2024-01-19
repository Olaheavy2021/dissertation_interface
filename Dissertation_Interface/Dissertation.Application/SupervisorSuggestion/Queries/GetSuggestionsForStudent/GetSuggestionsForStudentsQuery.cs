using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.SupervisorSuggestion.Queries.GetSuggestionsForStudent;

public sealed record GetSuggestionsForStudentsQuery : IRequest<ResponseDto<List<GetSupervisorSuggestion>>>;