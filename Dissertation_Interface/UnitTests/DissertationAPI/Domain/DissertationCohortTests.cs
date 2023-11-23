using Dissertation.Application.DTO.Request;
using Dissertation.Domain.Entities;
using Dissertation.Domain.Enums;
using FluentAssertions;
using UnitTests.DissertationAPI.Mocks;

namespace UnitTests.DissertationAPI.Domain;

public class DissertationCohortTests
{
    [Test]
    public void CreateDissertationCohort_Returns_Valid_Data()
    {
        //Arrange
        CreateDissertationCohortRequest request = DissertationCohortMocks.GetSuccessfulRequest(DateTime.UtcNow, DateTime.UtcNow);

        //Act
        var result = DissertationCohort.Create( request.EndDate, request.StartDate, request.SupervisionChoiceDeadline, request.AcademicYearId, DissertationConfigStatus.Active);

        //Assert
        result.StartDate.Should().Be(request.StartDate.Date);
        result.EndDate.Should().Be(request.EndDate.Date);
        result.Status.Should().Be( DissertationConfigStatus.Active);
        result.SupervisionChoiceDeadline.Should().Be( request.SupervisionChoiceDeadline);
        result.AcademicYearId.Should().Be( request.AcademicYearId);
    }
}