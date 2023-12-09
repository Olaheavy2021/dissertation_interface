using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Course.Commands.DisableCourse;

public sealed record DisableCourseCommandHandler : IRequestHandler<DisableCourseCommand, ResponseDto<GetCourse>>
{
    private readonly IAppLogger<DisableCourseCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public DisableCourseCommandHandler(IAppLogger<DisableCourseCommandHandler> logger, IUnitOfWork db,
        IMapper mapper)
    {
        this._logger = logger;
        this._mapper = mapper;
        this._db = db;
    }

    public async Task<ResponseDto<GetCourse>> Handle(DisableCourseCommand request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetCourse>();

        //fetch the course from the database
        Domain.Entities.Course? course =
            await this._db.CourseRepository.GetFirstOrDefaultAsync(a => a.Id == request.CourseId);

        if (course == null)
        {
            this._logger.LogError("No Course found with {ID}", request.CourseId);
            throw new NotFoundException(nameof(Domain.Entities.Course), request.CourseId);
        }

        if (course.Status.Equals(DissertationConfigStatus.InActive))
        {
            response.IsSuccess = false;
            response.Message = "Course is disabled already. Invalid Request";

            return response;
        }

        course.Status = DissertationConfigStatus.InActive;
        this._db.CourseRepository.Update(course);
        await this._db.SaveAsync(cancellationToken);

        GetCourse mappedCourse = this._mapper.Map<GetCourse>(course);

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedCourse;

        return response;
    }
}