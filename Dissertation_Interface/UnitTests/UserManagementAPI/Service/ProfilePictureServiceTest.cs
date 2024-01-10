using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shared.BlobStorage;
using Shared.DTO;
using Shared.Settings;
using UnitTests.UserManagementAPI.Mocks;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Service;

namespace UnitTests.UserManagementAPI.Service;

public class ProfilePictureServiceTest
{
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<ILogger<ProfilePictureService>> _mockLogger = null!;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor = null!;
    private Mock<IBlobRepository> _mockBlobRepository = null!;
    private IOptions<BlobStorageSettings> _blobStorageSettings = null!;
    private ProfilePictureService _service = null!;

    // Helper method to setup HttpContext with UserId
    private void SetupHttpContext(string email, string userId)
    {
        var mockHttpContext = new Mock<HttpContext>();
        var items = new Dictionary<object, object>
        {
            { "Email", email },
            { "UserId", userId }
        };

        mockHttpContext.Setup(c => c.Items).Returns(items);
        this._mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);
    }

    [SetUp]
    public void Setup()
    {
        this._mockUnitOfWork = new Mock<IUnitOfWork>();
        this._mockLogger = new Mock<ILogger<ProfilePictureService>>();
        this._mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        this._mockBlobRepository = new Mock<IBlobRepository>();
        this._blobStorageSettings = Options.Create(new BlobStorageSettings()); // Populate with necessary settings

        this._service = new ProfilePictureService(this._mockUnitOfWork.Object, this._mockLogger.Object, this._mockHttpContextAccessor.Object, this._mockBlobRepository.Object, this._blobStorageSettings);
    }

    [Test]
    public async Task UploadProfilePicture_ReturnsSuccessResponse_WhenValid()
    {
        // Arrange
        // Create a mock of IFormFile if needed
        var mockFormFile = new Mock<IFormFile>();
        mockFormFile.Setup(_ => _.FileName).Returns("testfile.png");
        mockFormFile.Setup(_ => _.Length).Returns(100);
        SetupHttpContext("test@example.com", "12345");
        var request = new ProfilePictureUploadRequestDto
        {
            FirstName = "abcde",
            LastName = "abcde",
            File = mockFormFile.Object
        };
        ApplicationUser applicationUser = TestData.User;
        applicationUser.ProfilePicture = TestData.ProfilePicture;
        var cancellationToken = new CancellationToken();

        this._mockUnitOfWork.Setup(u => u.ApplicationUserRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), null, x => x.ProfilePicture!))
            .ReturnsAsync(applicationUser);
        this._mockUnitOfWork.Setup(u => u.ProfilePictureRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<ProfilePicture, bool>>>(), null))
            .ReturnsAsync(TestData.ProfilePicture);
        this._mockBlobRepository.Setup(b => b.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()))
            .ReturnsAsync(new BlobResponseDto
            {
                Blob = new BlobDto()
                {
                    ContentType = "shsh",
                    Name = "shshs",
                    Uri = "ksksk"
                }
            });

        // Act
        ResponseDto<string> result = await this._service.UploadProfilePicture(request, cancellationToken);
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Profile Picture Updated Successfully"));
        });
    }
}