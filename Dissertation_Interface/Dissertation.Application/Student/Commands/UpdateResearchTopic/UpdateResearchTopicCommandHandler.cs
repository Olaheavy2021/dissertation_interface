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

namespace Dissertation.Application.Student.Commands.UpdateResearchTopic;

public class UpdateResearchTopicCommandHandler : IRequestHandler<UpdateResearchTopicCommand, ResponseDto<StudentDto>>
{
    private readonly IAppLogger<UpdateResearchTopicCommand> _logger;
    private readonly IUnitOfWork _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHtmlSanitizerService _htmlSanitizerService;
    private readonly IMapper _mapper;

    public UpdateResearchTopicCommandHandler(IAppLogger<UpdateResearchTopicCommand> logger, IUnitOfWork db,
        IHttpContextAccessor httpContextAccessor, IHtmlSanitizerService htmlSanitizerService, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._httpContextAccessor = httpContextAccessor;
        this._htmlSanitizerService = htmlSanitizerService;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<StudentDto>> Handle(UpdateResearchTopicCommand request,
        CancellationToken cancellationToken)
    {
        //fetch the student from the database
        var userId = this._httpContextAccessor.HttpContext?.Items["UserId"] as string;
        if (userId == null)
        {
            this._logger.LogError("Invalid token passed to Update Research Topic for Student");
            throw new NotFoundException("HttpContext", "UserId");
        }
        Domain.Entities.Student? student = await this._db.StudentRepository.GetFirstOrDefaultAsync(a => a.UserId == userId, includes: x => x.Course);
        if (student == null)
        {
            this._logger.LogError($"No Student found with {userId}", userId);
            throw new NotFoundException(nameof(Domain.Entities.Student), userId);
        }

        //sanitize the input
        var sanitizedResearchTopic = this._htmlSanitizerService.Sanitize(request.ResearchTopic);
        student.ResearchTopic = sanitizedResearchTopic;

        this._db.StudentRepository.Update(student);
        await this._db.SaveAsync(cancellationToken);
        StudentDto mappedStudent = this._mapper.Map<StudentDto>(student);
        return new ResponseDto<StudentDto>
        {
            IsSuccess = true, Message = SuccessMessages.DefaultSuccess, Result = mappedStudent
        };
    }
}