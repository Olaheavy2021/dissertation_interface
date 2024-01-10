using UserManagement_API.Data.Models;

namespace UnitTests.UserManagementAPI.Models;

public class ProfilePictureTests
{
    private const string ImageData = "imageData";
    private const string Name = "TestName";
    private const string ContentType = "image/jpeg";
    private const string UserId = "user123";

    [Test]
    public void Create_ShouldInitializePropertiesCorrectly()
    {
        var profilePicture = ProfilePicture.Create(ImageData, Name, ContentType, UserId);
        Assert.Multiple(() =>
        {
            Assert.That(profilePicture.ImageData, Is.EqualTo(ImageData));
            Assert.That(profilePicture.Name, Is.EqualTo(Name));
            Assert.That(profilePicture.ContentType, Is.EqualTo(ContentType));
            Assert.That(profilePicture.UserId, Is.EqualTo(UserId));
        });
    }
}