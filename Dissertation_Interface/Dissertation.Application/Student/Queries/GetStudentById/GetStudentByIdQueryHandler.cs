using System.Linq.Expressions;
using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Interfaces;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.Student.Queries.GetStudentById;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, ResponseDto<GetStudent>>
{
    private readonly IAppLogger<GetStudentByIdQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;
    private readonly IUserApiService _userApiService;

    public GetStudentByIdQueryHandler(IAppLogger<GetStudentByIdQueryHandler> logger, IUnitOfWork db, IMapper mapper, IUserApiService userApiService)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
        this._userApiService = userApiService;
    }
    public async Task<ResponseDto<GetStudent>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to retrieve a student by Id - {userId}", request.Id);

        // Fetch user details
        ResponseDto<GetUserDto> userResponse = await this._userApiService.GetUserByUserId(request.Id);
        if (!userResponse.IsSuccess || userResponse.Result == null)
        {
            return new ResponseDto<GetStudent>
            {
                IsSuccess = false,
                Message = userResponse.Message ?? "User not found."
            };
        }

        // Fetch student details
        Domain.Entities.Student? student = await this._db.StudentRepository.GetFirstOrDefaultAsync(
            x => x.UserId == request.Id,
            includes: new Expression<Func<Domain.Entities.Student, object>>[]
            {
                u => u.Course,
                u => u.DissertationCohort,
                u => u.ResearchProposal
            });

        if (student == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Student), request.Id);
        }

        // Map student details and prepare the response
        StudentDto mappedStudent = this._mapper.Map<StudentDto>(student);
        return new ResponseDto<GetStudent>
        {
            IsSuccess = true,
            Message = SuccessMessages.DefaultSuccess,
            Result = new GetStudent
            {
                UserDetails = userResponse.Result,
                StudentDetails = mappedStudent
            }
        };
    }
}