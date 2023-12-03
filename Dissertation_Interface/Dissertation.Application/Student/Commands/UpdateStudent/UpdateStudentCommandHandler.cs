using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Student.Commands.UpdateStudent;

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, ResponseDto<UserDto>>
{
    private readonly IAppLogger<UpdateStudentCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly IUserApiService _userApiService;

    public UpdateStudentCommandHandler(IAppLogger<UpdateStudentCommandHandler> logger, IUnitOfWork db, IMapper mapper, IUserApiService userApiService)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
        this._userApiService = userApiService;
    }
    public async Task<ResponseDto<UserDto>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        //fetch the student invite from the database
        Domain.Entities.Student? student = await this._db.StudentRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id);
        if (student == null)
        {
            this._logger.LogError("No Student found with {ID}", request.Id);
            throw new NotFoundException(nameof(Domain.Entities.Student), request.Id);
        }

        //update on the user api.
        var userApiRequest = new EditStudentRequestDto()
        {
            FirstName = request.FirstName,
            StudentId = request.StudentId,
            LastName = request.LastName,
            UserId = student.UserId,
            CourseId = request.CourseId

        };
       ResponseDto<UserDto> userResponse = await this._userApiService.EditStudent(userApiRequest);

        if (!userResponse.IsSuccess)
        {
            return new ResponseDto<UserDto> { IsSuccess = false, Message = userResponse.Message };
        }

        //update the department if necessary
        if (student.CourseId == request.CourseId)
            return new ResponseDto<UserDto>()
            {
                IsSuccess = true, Message = SuccessMessages.DefaultSuccess, Result = userResponse.Result
            };

        student.CourseId = request.CourseId;
        this._db.StudentRepository.Update(student);
        await this._db.SaveAsync(cancellationToken);

        return new ResponseDto<UserDto>
        {
            IsSuccess = true, Message = SuccessMessages.DefaultSuccess, Result = userResponse.Result
        };
    }
}