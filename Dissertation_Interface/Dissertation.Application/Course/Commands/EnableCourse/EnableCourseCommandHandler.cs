using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Enums;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Course.Commands.EnableCourse;

public class EnableCourseCommandHandler : IRequestHandler<EnableCourseCommand, ResponseDto<GetCourse>>
{
    private readonly IAppLogger<EnableCourseCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public EnableCourseCommandHandler(IAppLogger<EnableCourseCommandHandler> logger, IUnitOfWork db,
        IMapper mapper)
    {
        this._logger = logger;
        this._mapper = mapper;
        this._db = db;
    }

    public async Task<ResponseDto<GetCourse>> Handle(EnableCourseCommand request, CancellationToken cancellationToken)
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

        if (course.Status.Equals(DissertationConfigStatus.Active))
        {
            response.IsSuccess = false;
            response.Message = "Course is enabled already. Invalid Request";

            return response;
        }

        course.Status = DissertationConfigStatus.Active;
        this._db.CourseRepository.Update(course);
        await this._db.SaveAsync(cancellationToken);

        GetCourse mappedCourse = this._mapper.Map<GetCourse>(course);

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedCourse;

        return response;
    }
}