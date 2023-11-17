using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Helpers;
using Shared.Logging;

namespace Dissertation.Application.Course.Queries.GetListOfCourse;

public class GetListOfCourseQueryHandler : IRequestHandler<GetListOfCourseQuery,ResponseDto<PaginatedCourseListDto>>
{
    private readonly IAppLogger<GetListOfCourseQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetListOfCourseQueryHandler(IAppLogger<GetListOfCourseQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._db = db;
        this._logger = logger;
        this._mapper = mapper;
    }

    public Task<ResponseDto<PaginatedCourseListDto>> Handle(GetListOfCourseQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<PaginatedCourseListDto>();
        this._logger.LogInformation("Attempting to retrieve list of Course");
        PagedList<Domain.Entities.Course> course =  this._db.CourseRepository.GetListOfCourse(request.Parameters);

        var mappedCourse = new PagedList<GetCourse>(
            course.Select(MapToCourseDto).ToList(),
            course.TotalCount,
            course.CurrentPage,
            course.PageSize
        );

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = new PaginatedCourseListDto()
        {
            Data = mappedCourse,
            TotalCount = mappedCourse.TotalCount,
            PageSize = mappedCourse.PageSize,
            CurrentPage = mappedCourse.CurrentPage,
            TotalPages = mappedCourse.TotalPages,
            HasNext = mappedCourse.HasNext,
            HasPrevious = mappedCourse.HasPrevious
        };

        return Task.FromResult(response);
    }

    private  GetCourse MapToCourseDto(
        Domain.Entities.Course course)
    {
        GetDepartment mappedDepartment = this._mapper.Map<GetDepartment>(course.Department);
        return new GetCourse
        {
            Id = course.Id,
            Status = course.Status,
            Department = mappedDepartment,
            Name = course.Name,
            CreatedAt = course.CreatedAt,
            CreatedBy = course.CreatedBy,
            UpdatedAt = course.UpdatedAt,
            UpdatedBy = course.UpdatedBy
        };
    }
}