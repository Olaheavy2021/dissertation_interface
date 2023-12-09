using Dissertation.Application.DTO.Request;
using Dissertation.Domain.Entities;
using FluentAssertions;
using Shared.Enums;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Domain;

public class DepartmentTests
{
    [Test]
    public void CreateDepartment_Returns_Valid_Data()
    {
        //Arrange
        CreateDepartmentRequest request = DepartmentMock.GetSuccessfulRequest();

        //Act
        var result = Department.Create(request.Name);

        //Assert
        result.Name.Should().Be(request.Name);
        result.Status.Should().Be(DissertationConfigStatus.Active);
    }
}