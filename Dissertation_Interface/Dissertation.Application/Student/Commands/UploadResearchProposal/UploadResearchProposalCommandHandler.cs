using Dissertation.Domain.Entities;
using Dissertation.Infrastructure.Persistence.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.BlobStorage;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Settings;

namespace Dissertation.Application.Student.Commands.UploadResearchProposal;

public class UploadResearchProposalCommandHandler : IRequestHandler<UploadResearchProposalCommand, ResponseDto<string>>
{
    private readonly IUnitOfWork _db;
    private readonly ILogger<UploadResearchProposalCommandHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBlobRepository _blobRepository;
    private readonly BlobStorageSettings _blobStorageSettings;

    public UploadResearchProposalCommandHandler(IUnitOfWork db, ILogger<UploadResearchProposalCommandHandler> logger, IHttpContextAccessor httpContextAccessor, IBlobRepository blobRepository, IOptions<BlobStorageSettings>  blobStorageSettings)
    {
        this._db = db;
        this._logger = logger;
        this._httpContextAccessor = httpContextAccessor;
        this._blobRepository = blobRepository;
        this._blobStorageSettings = blobStorageSettings.Value;
    }

    public async Task<ResponseDto<string>> Handle(UploadResearchProposalCommand request, CancellationToken cancellationToken)
    {
        //get the user from the token
        var userId = this._httpContextAccessor.HttpContext?.Items["UserId"] as string;
        if (string.IsNullOrEmpty(userId))
        {
            return new ResponseDto<string> { Message = "Invalid Request", IsSuccess = false };
        }

        Domain.Entities.Student? student =
            await this._db.StudentRepository.GetFirstOrDefaultAsync(a => a.UserId == userId,
                includes: x => x.ResearchProposal);
        this._logger.LogInformation("Fetching details of this student with userId from the database - {0}", userId);
        if (student == null) throw new NotFoundException(nameof(Domain.Entities.Student), userId);


        var extension = ValidateUploadedFile(request);
        var blobName = $"{userId}{extension}";
        BlobResponseDto blobResponse = await this._blobRepository.UploadAsync(blobName,
            this._blobStorageSettings.ResearchProposalContainer,
            request.File);

        if (blobResponse.Error)
            return new ResponseDto<string>()
            {
                IsSuccess = blobResponse.Error,
                Message = blobResponse.Status,
                Result = ErrorMessages.DefaultError
            };

        if (string.IsNullOrEmpty(blobResponse.Blob.Uri) || string.IsNullOrEmpty(blobResponse.Blob.Name))
            return new ResponseDto<string>()
            {
                IsSuccess = false,
                Message = "Invalid response from the Blob API",
                Result = ErrorMessages.DefaultError
            };

        //update or create a record in the profile picture table as the case may be
        blobResponse.Blob.ContentType = extension;
        ResearchProposal? researchProposal =
            await this._db.ResearchProposalRepository.GetFirstOrDefaultAsync(x => x.StudentId == student.Id);
        if (researchProposal == null)
        {
            //create a new record for the research proposal
            await AddResearchProposal(blobResponse, student.Id, cancellationToken);
        }
        else
        {
            //update the research proposal
            await UpdateResearchProposal(student, blobResponse, cancellationToken);
        }

        return new ResponseDto<string>
        {
            IsSuccess = true,
            Message = "Research Proposal Updated Successfully",
            Result = SuccessMessages.DefaultSuccess
        };
    }

    private static string ValidateUploadedFile(UploadResearchProposalCommand request)
    {
        //validate the uploaded image
        const long maxFileSize = 5 * 1024 * 1024;
        string[] permittedExtensions = { ".pdf"};

        if (request.File == null)
        {
            throw new BadRequestException("No file was uploaded");
        }

        if (request.File.Length == 0)
        {
            throw new BadRequestException("No file was uploaded");
        }

        if (request.File.Length > maxFileSize)
            throw new BadRequestException("File size exceeds the permissible limit of 5Mb");

        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
        {
            throw new BadRequestException("Invalid file type.");
        }

        return extension;
    }

    private async Task AddResearchProposal(BlobResponseDto blobResponse, long studentId, CancellationToken cancellationToken)
    {
        var researchProposal = ResearchProposal.Create(
            blobResponse.Blob.Uri!,
            blobResponse.Blob.Name!,
            blobResponse.Blob.ContentType!,
            studentId
        );
        await this._db.ResearchProposalRepository.AddAsync(researchProposal);
        await this._db.SaveAsync(cancellationToken);
    }

    private async Task UpdateResearchProposal(Domain.Entities.Student student, BlobResponseDto blobResponse, CancellationToken cancellationToken)
    {
        student.ResearchProposal.Name = blobResponse.Blob.Name!;
        student.ResearchProposal.ImageData = blobResponse.Blob.Uri!;
        student.ResearchProposal.ContentType = blobResponse.Blob.ContentType!;

        this._db.StudentRepository.Update(student);
        await this._db.SaveAsync(cancellationToken);
    }
}