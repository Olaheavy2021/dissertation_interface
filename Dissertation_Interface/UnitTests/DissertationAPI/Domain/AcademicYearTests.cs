using Dissertation.Application.DTO;
using Dissertation.Domain.Entities;
using FluentAssertions;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Domain;

public class AcademicYearTests
{
    [Test]
    public void CreateAcademicYear_Returns_Valid_Data()
    {
        //Arrange
        CreateAcademicYearRequest request = AcademicYearMocks.GetSuccessfulRequest();

        //Act
        var result = AcademicYear.Create(request.StartDate, request.EndDate);

        //Assert
        result.StartDate.Should().Be(request.StartDate.Date);
        result.EndDate.Should().Be(request.EndDate.Date);
    }
}