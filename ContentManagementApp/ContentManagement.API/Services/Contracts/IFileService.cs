namespace ContentManagement.API.Services.Contracts
{
    public interface IFileService
    {
        Task<byte[]?> GetFileAsByteArrayAsync(string filePath);
        Task<Stream?> GetFileAsStreamAsync(string filePath);
        Task<bool> SaveFileFromByteArray(byte[] file, string filePath);
        Task<bool> SaveFileFromStream(Stream stream, string filePath);
        public void CreateSubFolders(string filePath);
        public string CreateFullPath(string filePath);

    }
}
