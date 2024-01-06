using Dissertation.Application.DTO.Response;
using Dissertation.Application.Utility;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Supervisor.Commands.UpdateResearchArea;

public class UpdateResearchAreaCommandHandler : IRequestHandler<UpdateResearchAreaCommand, ResponseDto<SupervisorDto>>
{
    private readonly IAppLogger<UpdateResearchAreaCommand> _logger;
    private readonly IUnitOfWork _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHtmlSanitizerService _htmlSanitizerService;
    private readonly IMapper _mapper;

    public UpdateResearchAreaCommandHandler(IAppLogger<UpdateResearchAreaCommand> logger, IUnitOfWork db,
        IHttpContextAccessor httpContextAccessor, IHtmlSanitizerService htmlSanitizerService, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._httpContextAccessor = httpContextAccessor;
        this._htmlSanitizerService = htmlSanitizerService;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<SupervisorDto>> Handle(UpdateResearchAreaCommand request,
        CancellationToken cancellationToken)
    {
        //fetch the supervision invite from the database
        if (this._httpContextAccessor.HttpContext?.Items["UserId"] is not string userId)
        {
            this._logger.LogError("Invalid token passed to Update Research Area for Supervisor");
            throw new NotFoundException("HttpContext", "UserId");
        }
        Domain.Entities.Supervisor? supervisor = await this._db.SupervisorRepository.GetFirstOrDefaultAsync(a => a.UserId == userId, includes: x => x.Department);
        if (supervisor == null)
        {
            this._logger.LogError($"No Supervisor found with {userId}", userId);
            throw new NotFoundException(nameof(Domain.Entities.Supervisor), userId);
        }

        //sanitize the input
        var sanitizedResearchArea = this._htmlSanitizerService.Sanitize(request.ResearchArea);
        supervisor.ResearchArea = sanitizedResearchArea;

        this._db.SupervisorRepository.Update(supervisor);
        await this._db.SaveAsync(cancellationToken);
        SupervisorDto mappedSupervisor = this._mapper.Map<SupervisorDto>(supervisor);
        return new ResponseDto<SupervisorDto>()
        {
            IsSuccess = true,
            Message = SuccessMessages.DefaultSuccess,
            Result = mappedSupervisor
        };
    }
}