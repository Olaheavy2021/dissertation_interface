using Dissertation.Domain.Enums;

namespace Dissertation.Application.DTO.Response;

public class GetDepartment
{
    public long Id { get; set; }

    public string Name { get; set; } = default!;

    public DissertationConfigStatus Status { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

}