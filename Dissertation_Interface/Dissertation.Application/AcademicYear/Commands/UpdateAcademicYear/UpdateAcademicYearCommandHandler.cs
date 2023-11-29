using Dissertation.Application.DTO.Response;
using Dissertation.Infrastructure.Persistence.IRepository;
using MapsterMapper;
using MediatR;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Logging;

namespace Dissertation.Application.AcademicYear.Commands.UpdateAcademicYear;

public class UpdateAcademicYearCommandHandler : IRequestHandler<UpdateAcademicYearCommand, ResponseDto<GetAcademicYear>>
{
    private readonly IAppLogger<UpdateAcademicYearCommandHandler> _logger;
    private readonly IUnitOfWork _db;
    private readonly IMapper _mapper;

    public UpdateAcademicYearCommandHandler(IAppLogger<UpdateAcademicYearCommandHandler> logger, IUnitOfWork db, IMapper mapper)
    {
        this._logger = logger;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<ResponseDto<GetAcademicYear>> Handle(UpdateAcademicYearCommand request,
        CancellationToken cancellationToken)
    {
        var response = new ResponseDto<GetAcademicYear>();
        //fetch the academic year from the database
        Domain.Entities.AcademicYear? academicYear = await this._db.AcademicYearRepository.GetFirstOrDefaultAsync(a => a.Id == request.Id);
        if (academicYear == null)
        {
            this._logger.LogError("No Academic Year found with ID");
            throw new NotFoundException(nameof(AcademicYear), request.Id);
        }

        //update the database
        academicYear.StartDate = request.StartDate;
        academicYear.EndDate = request.EndDate;
        this._db.AcademicYearRepository.Update(academicYear);
        await this._db.SaveAsync(cancellationToken);

        GetAcademicYear mappedAcademicYear = this._mapper.Map<GetAcademicYear>(academicYear);
        mappedAcademicYear.UpdateStatus();

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;
        response.Result = mappedAcademicYear;
        return response;
    }
}