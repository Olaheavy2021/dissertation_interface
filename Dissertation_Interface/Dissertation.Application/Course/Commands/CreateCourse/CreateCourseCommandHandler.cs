using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Enums;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.Course.Commands.CreateCourse;

public class CreateCourseCommandHandler: IRequestHandler<CreateCourseCommand, ResponseDto<GetCourse>>
{
    private readonly IAppLogger<CreateCourseCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public CreateCourseCommandHandler(IAppLogger<CreateCourseCommandHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._mapper = mapper;
        this._db = db;
    }

    public async Task<ResponseDto<GetCourse>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Create Course for this {name}", request.Name);
        var response = new ResponseDto<GetCourse>();
        var course = Domain.Entities.Course.Create(request.Name, request.DepartmentId);

        await this._db.CourseRepository.AddAsync(course);
        await this._db.SaveAsync(cancellationToken);
        GetCourse mappedCourse = this._mapper.Map<GetCourse>(course);

        response.Message = "Course Created Successfully";
        response.Result = mappedCourse;
        response.IsSuccess = true;
        this._logger.LogInformation("Course created for this {name}", request.Name);
        return response;
    }
}