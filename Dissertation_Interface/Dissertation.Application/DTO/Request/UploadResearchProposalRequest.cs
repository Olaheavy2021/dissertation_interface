using Microsoft.AspNetCore.Http;

namespace Dissertation.Application.DTO.Request;

public class UploadResearchProposalRequest
{
    public IFormFile ResearchProposal { get; set; } = null!;
}