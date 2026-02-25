using Azure.Storage.Blobs;
using JobNexus.Interfaces;

namespace JobNexus.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        private readonly string _containerName;

        private readonly IConfiguration _configuration;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _blobServiceClient = new BlobServiceClient(_configuration["AZURE_STORAGE_CONNECTION_STRING"]);
            _containerName = _configuration["AZURE_STORAGE_CONTAINER"] ?? "";
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // Ensure container exists
            await containerClient.CreateIfNotExistsAsync();

            // Ensure the file has a valid name
            var fileName = Path.GetFileName(file.FileName);

            // Use a unique name for the blob to avoid conflicts
            var blobName = Path.GetFileNameWithoutExtension(fileName) +
                           "_" + Guid.NewGuid().ToString() +
                           Path.GetExtension(fileName);

            var blobClient = containerClient.GetBlobClient(blobName);

            // Upload the file stream directly to Azure Blob Storage
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString(); // Returns the URI of the uploaded blob
        }
    }
}
