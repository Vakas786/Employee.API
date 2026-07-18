using Azure.Storage.Blobs;
using Employee.API.Interfaces;

namespace Employee.API.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IConfiguration _configuration;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var connectionString =
                _configuration["AzureBlobStorage:ConnectionString"];

            var containerName =
                _configuration["AzureBlobStorage:ContainerName"];

            BlobContainerClient container =
                new BlobContainerClient(connectionString, containerName);

            await container.CreateIfNotExistsAsync();

            string fileName =
                Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            BlobClient blob =
                container.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, true);
            }

            return blob.Uri.ToString();
        }
    }
}