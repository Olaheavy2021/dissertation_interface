using Microsoft.AspNetCore.Http;
using Shared.DTO;

namespace Shared.BlobStorage;

public interface IBlobRepository
{
    /// <summary>
    /// This method uploads a file submitted with the request
    /// </summary>
    /// <param name="blobName"></param>
    /// <param name="containerName"></param>
    /// <param name="file">File for upload</param>
    /// <returns>Blob with status</returns>
    Task<BlobResponseDto> UploadAsync(string blobName, string containerName, IFormFile file);

    /// <summary>
    /// This method downloads a file with the specified filename
    /// </summary>
    /// <param name="blobName"></param>
    /// <param name="containerName"></param>
    /// <returns>Blob</returns>
    Task<BlobDto> DownloadAsync(string blobName, string containerName);

    /// <summary>
    /// This method deleted a file with the specified filename
    /// </summary>
    /// <param name="blobName"></param>
    /// <param name="containerName"></param>
    /// <returns>Blob with status</returns>
    Task<BlobResponseDto> DeleteAsync(string blobName, string containerName);
}