using Shared.DTO;

namespace Dissertation.Application.DTO.Response;

public class GetStudent
{
    public StudentDto? StudentDetails { get; set; }
    public GetUserDto? UserDetails { get; set; }
}