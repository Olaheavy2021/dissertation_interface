using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.DTO;

namespace Shared.BlobStorage;

public class BlobRepository : IBlobRepository
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<BlobRepository> _logger;


    public BlobRepository(BlobServiceClient blobServiceClient, ILogger<BlobRepository> logger)
    {
        this._blobServiceClient = blobServiceClient;
        this._logger = logger;
    }

    public async Task<BlobDto> DownloadAsync(string blobName, string containerName)
    {
        BlobContainerClient? containerClient = this._blobServiceClient.GetBlobContainerClient(containerName);
        try
        {
            BlobClient? blobClient = containerClient.GetBlobClient(blobName);
            // Check if the file exists in the container
            if (await blobClient.ExistsAsync())
            {
                Stream? data = await blobClient.OpenReadAsync();
                Stream blobContent = data;

                // Download the file details async
                Response<BlobDownloadResult>? content = await blobClient.DownloadContentAsync();

                // Add data to variables in order to return a BlobDto
                var name = blobName;
                var contentType = content.Value.Details.ContentType;

                // Create new BlobDto with blob data from variables
                return new BlobDto { Content = blobContent, Name = name, ContentType = contentType };
            }
        }
        catch (RequestFailedException ex)
            when(ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            this._logger.LogError("File {blobName} was not found.", blobName);
        }

        // File does not exist, return null and handle that in requesting method
        return new BlobDto { Content = null, Name = blobName };
    }

    public async Task<BlobResponseDto> DeleteAsync(string blobName, string containerName)
    {
        BlobContainerClient? containerClient = this._blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient? blobClient = containerClient.GetBlobClient(blobName);
        try
        {
            // Delete the file
            await blobClient.DeleteIfExistsAsync();
        }
        catch (RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            // File did not exist, log and return new response to requesting method
            this._logger.LogError("File {blobName} was not found.", blobName);
            return new BlobResponseDto { Error = true, Status = $"File with name {blobName} not found." };
        }

        // Return a new BlobResponseDto to the requesting method
        return new BlobResponseDto { Error = false, Status = $"File: {blobName} has been successfully deleted." };
    }

    public async Task<BlobResponseDto> UploadAsync(string blobName, string containerName, IFormFile blob)
    {
        // Create new upload response object that we can return to the requesting method
        BlobResponseDto response = new();

        // Get a reference to a container named in appsettings.json and then create it
        BlobContainerClient? containerClient = this._blobServiceClient.GetBlobContainerClient(containerName);
        try
        {
            BlobClient? blobClient = containerClient.GetBlobClient(blobName);
            // Open a stream for the file we want to upload
            await using (Stream data = blob.OpenReadStream())
            {
                // Upload the file async
                await blobClient.UploadAsync(data, overwrite: true);
            }

            // Everything is OK and file got uploaded
            response.Status = $"File {blob.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = blobClient.Uri.AbsoluteUri;
            response.Blob.Name = blobClient.Name;
        }
        catch (RequestFailedException ex)
        {
            // Log error to console and create a new response we can return to the requesting method
            this._logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
            response.Status = $"Unexpected error: {ex.Message}.";
            response.Error = true;
            return response;
        }

        return response;
    }
}