using Dissertation.Application.DTO.Response;
using MediatR;
using Shared.DTO;

namespace Dissertation.Application.Student.Commands.UpdateResearchTopic;

public sealed record UpdateResearchTopicCommand(
    string ResearchTopic
    ) : IRequest<ResponseDto<StudentDto>>;