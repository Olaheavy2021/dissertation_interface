using Dissertation.Application.DTO.Request;
using Dissertation.Domain.Entities;

namespace UnitTests.DissertationAPI.Mocks;

public class CourseMocks
{
    public static CreateCourseRequest GetSuccessfulRequest() => new() { Name = "Computing", DepartmentId = 2};

    public static Course GetFirstOrDefaultResponse()
    {
        CreateCourseRequest response = GetSuccessfulRequest();
        return Course.Create(response.Name, 1);
    }
}