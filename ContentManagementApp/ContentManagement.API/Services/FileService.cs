using ContentManagement.API.Services.Contracts;
using ILogger = Serilog.ILogger;

namespace ContentManagement.API.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _config;
        private ILogger _logger;

        public FileService(IConfiguration config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<byte[]?> GetFileAsByteArrayAsync(string filePath)
        {
            try
            {
                string fullPath = CreateFullPath(filePath);
                byte[] file;
                bool exists = File.Exists(fullPath);
                if (exists)
                {
                    using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        file = new byte[fs.Length];
                        await fs.ReadAsync(file, 0, (int)fs.Length);

                    }
                    return file;
                }
                else
                {
                    throw new FileNotFoundException($"{fullPath}: Does Not Exist");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Stream?> GetFileAsStreamAsync(string filePath)
        {
            try
            {
                string fullPath = CreateFullPath(filePath);
                bool exists = File.Exists(fullPath);
                if (exists)
                {
                    using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1024))
                    {
                        return fs;
                    }
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> SaveFileFromByteArray(byte[] file, string filePath)
        {
            try
            {
                CreateSubFolders(filePath);

                string fullPath = CreateFullPath(filePath);

                using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    await fs.WriteAsync(file);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return false;
            }
        }

        public Task<bool> SaveFileFromStream(Stream stream, string filePath)
        {
            throw new NotImplementedException();
        }

        public void CreateSubFolders(string filePath)
        {
            string basePath = _config.GetValue<string>("VisualContentStorage")!;

            int index = filePath.LastIndexOf('\\');

            //extract the folders only
            string subFolders = filePath.Substring(0, index);

            string fullFolderPath = Path.Combine(basePath, subFolders);

            Directory.CreateDirectory(fullFolderPath);
        }

        public string CreateFullPath(string filePath)
        {
            string basePath = _config.GetValue<string>("VisualContentStorage")!;
            return Path.Combine(basePath, filePath);
        }
    }
}
