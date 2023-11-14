using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.AcademicYear.Queries.GetById;

public class GetAcademicYearByIdQueryHandler : IRequestHandler<GetAcademicYearByIdQuery, ResponseDto<GetAcademicYear>>
{
    private readonly IAppLogger<GetAcademicYearByIdQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetAcademicYearByIdQueryHandler(IAppLogger<GetAcademicYearByIdQueryHandler> logger,  IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetAcademicYear>> Handle(GetAcademicYearByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetAcademicYear>();
        this._logger.LogInformation("Attempting to retrieve an AcademicYear by ID {AcademicYearId}", request.AcademicYearId);
        Domain.Entities.AcademicYear? academicYear = await this._db.AcademicYearRepository.GetAsync(a => a.Id == request.AcademicYearId);
        if (academicYear is null)
        {
            this._logger.LogError("No Academic Year found with ID");
            throw new NotFoundException(nameof(AcademicYear), request.AcademicYearId);
        }

        GetAcademicYear mappedAcademicYear = this._mapper.Map<GetAcademicYear>(academicYear);

        this._logger.LogInformation("Successfully retrieved an Academic Year by ID {AcademicYearId}", academicYear.Id);
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedAcademicYear;
        return response;
    }
}