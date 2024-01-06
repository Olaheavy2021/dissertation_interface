using Dissertation.Infrastructure.DTO;

namespace Dissertation.Infrastructure.Services;

public interface IBatchUploadService
{
    Task ProcessStudentInvites(BulkUserUploadRequest request);

    Task ProcessSupervisorInvites(BulkUserUploadRequest request);
}