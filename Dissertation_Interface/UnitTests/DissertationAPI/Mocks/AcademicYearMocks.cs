using Bogus;
using Dissertation.Application.DTO;
using Dissertation.Domain.Entities;
using Dissertation.Domain.Enums;
using Shared.Helpers;

namespace UnitTests.DissertationAPI.Mocks;

public static class AcademicYearMocks
{
    public static CreateAcademicYearRequest GetSuccessfulRequest()
    {
        var currentYear = DateTime.Now.Year;
        var nextYear = currentYear + 1;

        var bogusDateGenerator = new Faker();

        // Generate StartDate in the current year
        DateTime startDate = bogusDateGenerator.Date.Between(new DateTime(currentYear, 1, 1), new DateTime(currentYear, 12, 31));

        // Generate EndDate in the next year
        DateTime endDate = bogusDateGenerator.Date.Between(new DateTime(nextYear, 1, 1), new DateTime(nextYear, 12, 31));

        return new CreateAcademicYearRequest() { StartDate = startDate, EndDate = endDate };

    }

    public static CreateAcademicYearRequest GetFailedRequest()
    {
        var currentYear = DateTime.Now.Year - 1;
        var nextYear = currentYear + 4;

        var bogusDateGenerator = new Faker();

        // Generate StartDate in the current year
        DateTime startDate = bogusDateGenerator.Date.Between(new DateTime(currentYear, 1, 1), new DateTime(currentYear, 12, 31));

        // Generate EndDate in the next year
        DateTime endDate = bogusDateGenerator.Date.Between(new DateTime(nextYear, 1, 1), new DateTime(nextYear, 12, 31));

        return new CreateAcademicYearRequest() { StartDate = startDate, EndDate = endDate };

    }

    public static AcademicYear GetFirstOrDefaultResponse()
    {
        CreateAcademicYearRequest response = GetSuccessfulRequest();
        return AcademicYear.Create(response.StartDate, response.EndDate);
    }

    public static PagedList<AcademicYear> GetPaginatedResponse()
    {
        AcademicYear academicYear = GetFirstOrDefaultResponse();
        var listOfAcademicYear = new List<AcademicYear>
        {
            academicYear
        };

        var response = new PagedList<AcademicYear>(listOfAcademicYear, 1, 1, 10);

        return response;
    }
}