using System.ComponentModel.DataAnnotations.Schema;
using Dissertation.Domain.DomainHelper;

namespace Dissertation.Domain.Entities;

public class Student : AuditableEntity<long>
{
    public long CourseId { get; set; }

    [ForeignKey("CourseId")]
    public Course Course { get; set; }

    public string? ProfilePicture { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public string? ResearchTopic { get; set; } = default!;

    public string? ResearchProposal { get; set; } = default!;

    public long DissertationCohortId { get; set; }

    [ForeignKey("DissertationCohortId")]
    public DissertationCohort DissertationCohort { get; set; }

    private Student(
        string userId,
        long courseId,
        long dissertationCohortId
    )
    {
        UserId = userId;
        CourseId = courseId;
        DissertationCohortId = dissertationCohortId;
    }

    public static Student Create(
        string userId,
        long courseId,
        long dissertationCohortId
    ) =>
        new(userId, courseId, dissertationCohortId);
}