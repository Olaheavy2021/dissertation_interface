using Dissertation.Application.DTO.Request;
using Dissertation.Domain.Entities;

namespace UnitTests.DissertationAPI.Mocks;

public class DepartmentMock
{
    public static CreateDepartmentRequest GetSuccessfulRequest() => new() { Name = "Computing" };

    public static Department GetFirstOrDefaultResponse()
    {
        CreateDepartmentRequest response = GetSuccessfulRequest();
        return Department.Create(response.Name);
    }
}