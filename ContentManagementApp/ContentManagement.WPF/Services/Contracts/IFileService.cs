using ContentManagement.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.WPF.Services.Contracts
{
    public interface IFileService
    {
        Task<byte[]?> GetImageByteAsync(string filePath);
        Task<Stream?> GetImageStreamAsync(string filePath);
        Task<bool> SaveImageAsByteAsync(string localFilePath, string destinationFilePath);
        Task<bool> SaveImageAsStream(string localFilePath, string destinationFilePath);
        Task<byte[]> GetByteArrayFromHttpSource(string address);
    }
}
