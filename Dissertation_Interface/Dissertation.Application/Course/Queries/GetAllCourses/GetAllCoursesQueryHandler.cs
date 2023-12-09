using Dissertation.Application.Course.Queries.GetListOfCourse;
using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Helpers;
using Shared.Logging;

namespace Dissertation.Application.Course.Queries.GetAllCourses;

public class GetAllCoursesQueryHandler: IRequestHandler<GetAllCoursesQuery, ResponseDto<IReadOnlyList<GetCourse>>>
{
    private readonly IAppLogger<GetAllCoursesQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetAllCoursesQueryHandler(IAppLogger<GetAllCoursesQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._db = db;
        this._logger = logger;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<IReadOnlyList<GetCourse>>> Handle(GetAllCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<IReadOnlyList<GetCourse>>();
        this._logger.LogInformation("Attempting to retrieve list of all Course");
        IReadOnlyList<Domain.Entities.Course> courses = await this._db.CourseRepository.GetAllAsync();

        IReadOnlyList<GetCourse> mappedCourse = this._mapper.Map<IReadOnlyList<GetCourse>>(courses);

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedCourse;

        return response;
    }
}