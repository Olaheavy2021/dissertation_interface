using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.DTO;

namespace Dissertation.Application.Student.Commands.UploadResearchProposal;

public sealed record UploadResearchProposalCommand(IFormFile File) : IRequest<ResponseDto<string>>;