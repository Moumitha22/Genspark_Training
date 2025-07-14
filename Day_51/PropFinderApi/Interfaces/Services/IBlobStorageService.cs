namespace PropFinderApi.Interfaces
{
    public interface IBlobStorageService
    {
        Task UploadFileToPathAsync(Stream fileStream, string blobPath);

        Task<Stream> DownloadFileAsync(string blobPath);

        Task DeleteFileAsync(string blobPath);

        Task<string> GetBlobUrlAsync(string blobPath);

        Task<string> GetContainerBaseUrlAsync();
    }
}
