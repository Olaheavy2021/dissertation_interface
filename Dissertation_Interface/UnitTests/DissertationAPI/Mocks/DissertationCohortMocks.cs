using Bogus;
using Dissertation.Application.DTO.Request;
using Dissertation.Domain.Entities;
using Dissertation.Domain.Enums;
using Shared.Helpers;

namespace UnitTests.DissertationAPI.Mocks;

public class DissertationCohortMocks
{
    public static CreateDissertationCohortRequest GetSuccessfulRequest(DateTime academicYearStartDate, DateTime academicYearEndDate)
    {
        var bogusDateGenerator = new Faker();
        var randomizer = new Randomizer();

        // Generate StartDate in the current year
        DateTime startDate = academicYearStartDate;

        // Generate EndDate in the next year
        DateTime endDate = academicYearEndDate.Add(TimeSpan.FromDays(-5));

        // Generate Supervision Deadline
        DateTime supervisionDeadline = bogusDateGenerator.Date.Between(new DateTime(startDate.Date.Year, startDate.Date.Month, startDate.Date.Day), new DateTime(endDate.Date.Year, endDate.Date.Month, endDate.Date.Day));

        return new CreateDissertationCohortRequest()
        {
            StartDate = startDate,
            EndDate = endDate,
            SupervisionChoiceDeadline = supervisionDeadline,
            AcademicYearId = randomizer.Long(min: 0, max: long.MaxValue)
        };
    }

    public static DissertationCohort GetFirstOrDefaultResponse()
    {
        AcademicYear academicYear = AcademicYearMocks.GetFirstOrDefaultResponse();
        CreateDissertationCohortRequest request = GetSuccessfulRequest(academicYear.StartDate, academicYear.EndDate);
        return DissertationCohort.Create(request.EndDate, request.StartDate, request.SupervisionChoiceDeadline,
            request.AcademicYearId, DissertationConfigStatus.Active);
    }

    public static PagedList<DissertationCohort> GetPaginatedResponse()
    {
        DissertationCohort dissertationCohort = GetFirstOrDefaultResponse();
        var listOfDissertationCohort = new List<DissertationCohort>();
        listOfDissertationCohort.Add(dissertationCohort);

        var response = new PagedList<DissertationCohort>(listOfDissertationCohort, 1, 1, 10);

        return response;
    }
}