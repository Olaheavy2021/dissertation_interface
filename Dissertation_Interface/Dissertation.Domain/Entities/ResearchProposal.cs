using Dissertation.Domain.DomainHelper;

namespace Dissertation.Domain.Entities;

public class ResearchProposal : AuditableEntity<long>
{
    public string ImageData { get; set; }

    public string Name { get; set; }

    public string ContentType { get; set; }

    // Foreign Key
    public long StudentId { get; set; }

    // Navigation Property
    public virtual Student Student { get; set; } = null!;

    private ResearchProposal(
        string imageData,
        string name,
        string contentType,
        long studentId
    )
    {
        ImageData = imageData;
        Name = name;
        ContentType = contentType;
        StudentId = studentId;
    }

    public static ResearchProposal Create(
        string imageData,
        string name,
        string contentType,
        long studentId) =>
        new(imageData, name, contentType, studentId);
}