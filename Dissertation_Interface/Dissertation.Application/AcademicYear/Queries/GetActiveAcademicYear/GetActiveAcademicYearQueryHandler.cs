using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Enums;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.AcademicYear.Queries.GetActiveAcademicYear;

public class GetActiveAcademicYearQueryHandler : IRequestHandler<GetActiveAcademicYearQuery, ResponseDto<GetAcademicYear>>
{
    private readonly IAppLogger<GetActiveAcademicYearQueryHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public GetActiveAcademicYearQueryHandler(IAppLogger<GetActiveAcademicYearQueryHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetAcademicYear>> Handle(GetActiveAcademicYearQuery request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetAcademicYear>();
        this._logger.LogInformation("Attempting to retrieve the active AcademicYear");
        Domain.Entities.AcademicYear? academicYear = await this._db.AcademicYearRepository.GetActiveAcademicYear();

        if (academicYear is null)
        {
            this._logger.LogError("No Academic Year found with an active status");
            throw new NotFoundException(nameof(AcademicYear), DissertationConfigStatus.Active);
        }

        GetAcademicYear mappedAcademicYear = this._mapper.Map<GetAcademicYear>(academicYear);
        mappedAcademicYear.UpdateStatus();

        this._logger.LogInformation("Successfully retrieved the active AcademicYear");
        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedAcademicYear;
        return response;
    }
}