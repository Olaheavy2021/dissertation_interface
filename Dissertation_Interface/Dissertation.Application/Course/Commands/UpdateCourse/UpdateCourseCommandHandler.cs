using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Course.Commands.UpdateCourse;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, ResponseDto<GetCourse>>
{
    private readonly IAppLogger<UpdateCourseCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public UpdateCourseCommandHandler(IAppLogger<UpdateCourseCommandHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetCourse>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetCourse>();
        //fetch the course from the database
        Domain.Entities.Course? course = await this._db.CourseRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id);
        if (course == null)
        {
            this._logger.LogError("No Course found with {ID}", request.Id);
            throw new NotFoundException(nameof(Domain.Entities.Course), request.Id);
        }

        //update the database
        course.Name = request.Name;
        course.DepartmentId = request.DepartmentId;
        this._db.CourseRepository.Update(course);
        await this._db.SaveAsync(cancellationToken);

        GetCourse mappedCourse = this._mapper.Map<GetCourse>(course);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedCourse;
        return response;
    }
}