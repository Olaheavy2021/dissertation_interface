using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Logging;

namespace Dissertation.Application.AcademicYear.Commands.CreateAcademicYear;

public class CreateAcademicYearCommandHandler : IRequestHandler<CreateAcademicYearCommand, ResponseDto<GetAcademicYear>>
{
    private readonly IAppLogger<CreateAcademicYearCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public CreateAcademicYearCommandHandler(IAppLogger<CreateAcademicYearCommandHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }
    public async Task<ResponseDto<GetAcademicYear>> Handle(CreateAcademicYearCommand request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Attempting to Create Academic year for this {startDate}", request.StartDate);
        var response = new ResponseDto<GetAcademicYear>();
        var academicYear = Domain.Entities.AcademicYear.Create(request.StartDate, request.EndDate);

        await this._db.AcademicYearRepository.AddAsync(academicYear);
        await this._db.SaveAsync(cancellationToken);

        GetAcademicYear mappedAcademicYear = this._mapper.Map<GetAcademicYear>(academicYear);
        mappedAcademicYear.UpdateStatus();

        response.Message = "Academic year initiated successfully";
        response.Result = mappedAcademicYear;
        response.IsSuccess = true;
        this._logger.LogInformation("Academic year created for this {startDate}", request.StartDate);
        return response;
    }
}