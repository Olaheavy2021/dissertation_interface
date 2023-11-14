using Dissertation.Domain.Enums;

namespace Dissertation.Application.DTO.Response;

public class GetAcademicYear
{
    public long Id { get; set; }
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DissertationConfigStatus Status { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}