// using Azure.Storage.Blobs;
// using Azure.Storage.Blobs.Models;
// using Microsoft.AspNetCore.StaticFiles;
// using PropFinderApi.Interfaces;

// namespace PropFinderApi.Services
// {
//     public class BlobStorageService : IBlobStorageService
//     {
//         private readonly BlobContainerClient _containerClient;

//         public BlobStorageService(IConfiguration configuration)
//         {
//             var containerSasUrl = configuration["AzureBlob:ContainerSasUrl"];
//             _containerClient = new BlobContainerClient(new Uri(containerSasUrl));
//         }


//        public async Task UploadFileToPathAsync(Stream fileStream, string blobPath)
//         {
//             var blobClient = _containerClient.GetBlobClient(blobPath);

//             await blobClient.DeleteIfExistsAsync();

//             // Detect content type
//             var provider = new FileExtensionContentTypeProvider();
//             string contentType = "application/octet-stream";
//             if (provider.TryGetContentType(blobPath, out var detectedType))
//                 contentType = detectedType;

//             // Upload with content type
//             await blobClient.UploadAsync(fileStream, new BlobUploadOptions
//             {
//                 HttpHeaders = new BlobHttpHeaders
//                 {
//                     ContentType = contentType
//                 }
//             });
//         }


       
//        public string GetBlobUrl(string blobPath)
//         {
//             // Ensure blobPath has no leading slashes
//             blobPath = blobPath.TrimStart('/');

//             // Base container URL without query (e.g., https://<account>.blob.core.windows.net/moumitha-images)
//             var baseUrl = _containerClient.Uri.GetLeftPart(UriPartial.Path);

//             // The query string part (?sp=...&sig=...)
//             var sasToken = _containerClient.Uri.Query;

//             // Construct full URL
//             return $"{baseUrl}/{blobPath}{sasToken}";
//         }

//         public string GetContainerBaseUrl()
//         {
//             // e.g. returns "https://youraccount.blob.core.windows.net/your-container"
//             return _containerClient.Uri.GetLeftPart(UriPartial.Path);
//         }



//         public async Task DeleteFileAsync(string blobPath)
//         {
//             var blobClient = _containerClient.GetBlobClient(blobPath);
//             await blobClient.DeleteIfExistsAsync();
//         }
//     }
// }


using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.StaticFiles;
using PropFinderApi.Interfaces;

namespace PropFinderApi.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private BlobContainerClient _containerClient;
        private readonly IConfiguration _configuration;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task UpdateContainerClientAsync()
        {
            if (_containerClient != null) return; 

            string keyVaultUrl = _configuration["AzureBlob:KeyVaultUrl"];
            var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

            KeyVaultSecret secret = await secretClient.GetSecretAsync("SasUrl");
            string sasUrl = secret.Value;

            _containerClient = new BlobContainerClient(new Uri(sasUrl));
        }

        public async Task UploadFileToPathAsync(Stream fileStream, string blobPath)
        {
            await UpdateContainerClientAsync();

            var blobClient = _containerClient.GetBlobClient(blobPath);
            await blobClient.DeleteIfExistsAsync();

            var provider = new FileExtensionContentTypeProvider();
            string contentType = provider.TryGetContentType(blobPath, out var detectedType)
                ? detectedType
                : "application/octet-stream";

            await blobClient.UploadAsync(fileStream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            });
        }

        public async Task<Stream> DownloadFileAsync(string blobPath)
        {
            await UpdateContainerClientAsync();

            var blobClient = _containerClient.GetBlobClient(blobPath);
            if (await blobClient.ExistsAsync())
            {
                var downloadInfo = await blobClient.DownloadStreamingAsync();
                return downloadInfo.Value.Content;
            }
            return null;
        }

        public async Task DeleteFileAsync(string blobPath)
        {
            await UpdateContainerClientAsync();

            var blobClient = _containerClient.GetBlobClient(blobPath);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> GetBlobUrlAsync(string blobPath)
        {
            await UpdateContainerClientAsync();

            blobPath = blobPath.TrimStart('/');
            var baseUrl = _containerClient.Uri.GetLeftPart(UriPartial.Path);
            var sasToken = _containerClient.Uri.Query;

            return $"{baseUrl}/{blobPath}{sasToken}";
        }

        public async Task<string> GetContainerBaseUrlAsync()
        {
            await UpdateContainerClientAsync();
            return _containerClient.Uri.GetLeftPart(UriPartial.Path);
        }
    }
}
