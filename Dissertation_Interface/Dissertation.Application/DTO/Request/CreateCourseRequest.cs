namespace Dissertation.Application.DTO.Request;

public class CreateCourseRequest
{
    public string Name { get; set; } = default!;

    public long DepartmentId { get; set; }
}