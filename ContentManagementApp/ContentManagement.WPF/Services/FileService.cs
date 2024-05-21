using ContentManagement.DTOs;
using ContentManagement.WPF.Services.Contracts;
using Microsoft.AspNetCore.Builder;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Log = Serilog.Log;

namespace ContentManagement.WPF.Services
{
    public class FileService : IFileService
    {
        private readonly IHttpClientService _httpClientService;

        public FileService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<byte[]?> GetImageByteAsync(string filePath)
        {
            try
            {
                //HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"file/GetFileAsByteArray/{filePath}");
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"file/GetFileAsByteArray");
                httpRequestMessage.Headers.Add("FilePath", filePath);
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.SendAsync(httpRequestMessage);
                //HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.GetAsync($"file/getfileasstream/{filePath}");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    FileByteArrayDTO image = await httpResponseMessage.Content.ReadFromJsonAsync<FileByteArrayDTO>();
                    if (image is not null)
                    {
                        return image.File;
                    }
                    return null;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }
        }

        public async Task<Stream?> GetImageStreamAsync(string filePath)
        {
            try
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "file/GetFileAsStream");
                httpRequestMessage.Headers.Add("FilePath", filePath);
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    return stream;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }
        }

        public async Task<byte[]> GetByteArrayFromHttpSource(string address)
        {
            HttpClient client = new HttpClient();

            try
            {
                HttpResponseMessage httpResponseMessage = await client.GetAsync(address, HttpCompletionOption.ResponseHeadersRead);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    byte[] image = await httpResponseMessage.Content.ReadAsByteArrayAsync();
                    return image;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return null;
            }
        }
        public static Stream DownloadFile(string TargetFile)
        {
            using WebClient MyWebClient = new WebClient();
            byte[] BytesFile = MyWebClient.DownloadData(TargetFile);

            MemoryStream iStream = new MemoryStream(BytesFile);

            return iStream;
        }
        public async Task<bool> SaveImageAsByteAsync(string localFilePath, string destinationFilePath)
        {
            try
            {
                byte[] file = await GetByteArray(localFilePath);
                FileByteArrayDTO fileByteArray = new FileByteArrayDTO()
                {
                    File = file,
                    FilePath = destinationFilePath
                };

                var response = await _httpClientService.HttpClient.PostAsJsonAsync("file/SaveFileAsByteArray", fileByteArray);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }


        }

        public async Task<bool> SaveImageAsStream(string localFilePath, string destinationFilePath)
        {
            try
            {
                await using Stream stream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, true);

                using StreamContent streamContent = new StreamContent(stream);

                HttpRequestMessage request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(_httpClientService.HttpClient.BaseAddress + "file/savefileasstream"),
                    Method = HttpMethod.Post,
                    Content = streamContent
                };

                request.Headers.Add("FilePath", destinationFilePath);

                HttpResponseMessage response = await _httpClientService.HttpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        private async Task<byte[]> GetByteArray(string localFilePath)
        {
            using (FileStream fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] file = new byte[fs.Length];
                await fs.ReadAsync(file);
                return file;
            }
        }
    }
}
