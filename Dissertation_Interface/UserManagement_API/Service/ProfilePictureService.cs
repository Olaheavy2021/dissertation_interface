using Microsoft.Extensions.Options;
using Shared.BlobStorage;
using Shared.Constants;
using Shared.DTO;
using Shared.Exceptions;
using Shared.Settings;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Service;

public class ProfilePictureService : IProfilePictureService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProfilePictureService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBlobRepository _blobRepository;
    private readonly BlobStorageSettings _blobStorageSettings;

    public ProfilePictureService(IUnitOfWork unitOfWork, ILogger<ProfilePictureService> logger, IHttpContextAccessor httpContextAccessor, IBlobRepository blobRepository, IOptions<BlobStorageSettings>  blobStorageSettings)
    {
        this._unitOfWork = unitOfWork;
        this._logger = logger;
        this._httpContextAccessor = httpContextAccessor;
        this._blobRepository = blobRepository;
        this._blobStorageSettings = blobStorageSettings.Value;
    }

    public async Task<ResponseDto<string>> UploadProfilePicture(ProfilePictureUploadRequestDto request, CancellationToken cancellationToken)
    {
        //get the user from the token
        var userId = this._httpContextAccessor.HttpContext?.Items["UserId"] as string;
        if (string.IsNullOrEmpty(userId))
        {
            return new ResponseDto<string> { Message = "Invalid Request", IsSuccess = false };
        }

        ApplicationUser? user =
            await this._unitOfWork.ApplicationUserRepository.GetFirstOrDefaultAsync(a => a.Id == userId,
                includes: x => x.ProfilePicture);
        this._logger.LogInformation("Fetching details of this user with userId from the database - {0}", userId);
        if (user == null) throw new NotFoundException(nameof(ApplicationUser), userId);

        if (request.File.Length == 0)
        {
           return await UpdateUserDetailsOnly(request, user, cancellationToken);
        }

        //validate the uploaded image
        var extension = ValidateUploadedFile(request);
        var blobName = $"{user.Id}{extension}";
        BlobResponseDto blobResponse = await this._blobRepository.UploadAsync(blobName,
            this._blobStorageSettings.ProfilePictureContainer,
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
        ProfilePicture? profilePicture =
            await this._unitOfWork.ProfilePictureRepository.GetFirstOrDefaultAsync(x => x.UserId == userId);
        if (profilePicture == null)
        {
            //create a new record for the profile picture
            await AddProfilePicture(blobResponse, userId);
            //update the user
            UpdateUser(request, user);
            //save changes
            await this._unitOfWork.SaveAsync(cancellationToken);
        }
        else
        {
            //update the user
            UpdateUserAndProfilePicture(request, user, blobResponse);
            //save changes
            await this._unitOfWork.SaveAsync(cancellationToken);
        }

        return new ResponseDto<string>()
        {
            IsSuccess = true,
            Message = "Profile Picture Updated Successfully",
            Result = SuccessMessages.DefaultSuccess
        };
    }

    private async Task AddProfilePicture(BlobResponseDto blobResponse, string userId)
    {
        var profilePicture = ProfilePicture.Create(
            blobResponse.Blob.Uri!,
            blobResponse.Blob.Name!,
            blobResponse.Blob.ContentType!,
            userId
        );
        await this._unitOfWork.ProfilePictureRepository.AddAsync(profilePicture);
    }

    private async Task UpdateProfilePicture(BlobResponseDto blobResponse, string userId)
    {
        ProfilePicture? profilePicture =
            await this._unitOfWork.ProfilePictureRepository.GetFirstOrDefaultAsync(x => x.UserId == userId);
        if (profilePicture == null) throw new NotFoundException(nameof(ProfilePicture), userId);
        profilePicture.Name = blobResponse.Blob.Name!;
        profilePicture.ImageData = blobResponse.Blob.Uri!;
        profilePicture.ContentType = blobResponse.Blob.ContentType!;

        this._unitOfWork.ProfilePictureRepository.Update(profilePicture);
    }

    private void UpdateUser(ProfilePictureUploadRequestDto request, ApplicationUser user)
    {
        if (!string.IsNullOrEmpty(request.LastName))
            user.LastName = request.LastName;

        if (!string.IsNullOrEmpty(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrEmpty(request.FirstName) || !string.IsNullOrEmpty(request.LastName))
            this._unitOfWork.ApplicationUserRepository.Update(user);
    }

    private void UpdateUserAndProfilePicture(ProfilePictureUploadRequestDto request, ApplicationUser user, BlobResponseDto blobResponse)
    {
        user.ProfilePicture.Name = blobResponse.Blob.Name!;
        user.ProfilePicture.ImageData = blobResponse.Blob.Uri!;
        user.ProfilePicture.ContentType = blobResponse.Blob.ContentType!;

        if (!string.IsNullOrEmpty(request.LastName))
            user.LastName = request.LastName;

        if (!string.IsNullOrEmpty(request.FirstName))
            user.FirstName = request.FirstName;

        this._unitOfWork.ApplicationUserRepository.Update(user);
    }

    private async Task<ResponseDto<string>> UpdateUserDetailsOnly(ProfilePictureUploadRequestDto request, ApplicationUser user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.FirstName) && string.IsNullOrEmpty(request.LastName))
        {
            return new ResponseDto<string>()
            {
                Message = "Invalid Request", IsSuccess = false, Result = ErrorMessages.DefaultError
            };
        }

        //attempt to update only the users
        UpdateUser(request, user);
        await this._unitOfWork.SaveAsync(cancellationToken);
        return new ResponseDto<string>()
        {
            IsSuccess = true,
            Message = "User Details Update Successfully",
            Result = SuccessMessages.DefaultSuccess
        };
    }

    private static string ValidateUploadedFile(ProfilePictureUploadRequestDto request)
    {
        //validate the uploaded image
        const long maxFileSize = 5 * 1024 * 1024;
        string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };

        if (request.File.Length > maxFileSize)
            throw new BadRequestException("File size exceeds the permissible limit of 5Mb");

        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
        {
            throw new BadRequestException("Invalid file type.");
        }

        return extension;
    }
}