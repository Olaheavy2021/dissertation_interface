using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Enums;
using Shared.Logging;

namespace Dissertation.Application.Course.Queries.GetListOfActiveCourse;

public class GetListOfActiveCoursesQueryHandler : IRequestHandler<GetListOfActiveCoursesQuery, ResponseDto<IEnumerable<GetCourse>>>
{
    private readonly IAppLogger<GetListOfActiveCoursesQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetListOfActiveCoursesQueryHandler(IAppLogger<GetListOfActiveCoursesQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._db = db;
        this._logger = logger;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<IEnumerable<GetCourse>>> Handle(GetListOfActiveCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<IEnumerable<GetCourse>>();
        this._logger.LogInformation("Attempting to retrieve list of active courses");

        IReadOnlyList<Domain.Entities.Course> courses = await this._db.CourseRepository.GetAllAsync(x => x.Status == DissertationConfigStatus.Active);
        IEnumerable<GetCourse> mappedCourses = this._mapper.Map<IEnumerable<GetCourse>>(courses);

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedCourses;

        return response;
    }
}