using Dissertation.Application.DTO.Request;
using Dissertation.Domain.Entities;
using FluentAssertions;
using Shared.Enums;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Domain;

public class CourseTests
{
    [Test]
    public void CreateCourse_Returns_Valid_Data()
    {
        //Arrange
        CreateCourseRequest request = CourseMocks.GetSuccessfulRequest();

        //Act
        var result = Course.Create(request.Name, request.DepartmentId);

        //Assert
        result.Name.Should().Be(request.Name);
        result.Status.Should().Be(DissertationConfigStatus.Active);
        result.DepartmentId.Should().Be(request.DepartmentId);
    }
}