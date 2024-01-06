namespace Dissertation.Infrastructure.DTO;

public class BulkUserUploadRequest
{
    public List<UserUploadRequest> Requests { get; set; } = null!;
    public string BatchUploadType { get; set; } = default!;
    public long ActiveCohortId { get; set; }
}