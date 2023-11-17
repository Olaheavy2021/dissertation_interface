using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Course.Queries.GetById;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, ResponseDto<GetCourse>>
{
    private readonly IAppLogger<GetCourseByIdQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetCourseByIdQueryHandler(IAppLogger<GetCourseByIdQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetCourse>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetCourse>();
        this._logger.LogInformation("Attempting to retrieve a Course by ID {CourseID}", request.CourseId);
        Domain.Entities.Course? course = await this._db.CourseRepository.GetAsync(a => a.Id == request.CourseId);
        if (course is null)
        {
            this._logger.LogError("No Course found with ID");
            throw new NotFoundException(nameof(Domain.Entities.Course), request.CourseId);
        }

        GetCourse mappedCourse = this._mapper.Map<GetCourse>(course);

        this._logger.LogInformation("Successfully retrieved an Course by ID {CourseID}", course.Id);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedCourse;
        return response;
    }
}