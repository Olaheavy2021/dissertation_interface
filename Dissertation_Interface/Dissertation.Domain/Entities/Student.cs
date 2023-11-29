using System.ComponentModel.DataAnnotations.Schema;

namespace Dissertation.Domain.Entities;

public class Student
{
    public long CourseId { get; set; }

    [ForeignKey("CourseId")]
    public Course Course { get; set; }

    public string? ProfilePicture { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public string? ResearchTopic { get; set; } = default!;

    public string? ResearchProposal { get; set; } = default!;
}