using ContentManagement.DTOs;
using ContentManagement.WPF.Services.Contracts;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Log = Serilog.Log;

namespace ContentManagement.WPF.Services
{
    public class AuthorVisualContentService : IAuthorVisualContentService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IFileService _fileService;
        private readonly IUserDetailService _userDetailService;

        public AuthorVisualContentService(IHttpClientService httpClientService,
                                          IFileService fileService,
                                          IUserDetailService userDetailService)
        {
            _httpClientService = httpClientService;
            _fileService = fileService;
            _userDetailService = userDetailService;
        }

        public async Task<AuthorVisualContentDTO?> AddAuthorVisualContent(AuthorVisualContentDTO? authorVisualContent, string localFilePath)
        {
            try
            {
                string relativePath = GenerateRelativePath(authorVisualContent!, localFilePath);
                authorVisualContent!.FileName = relativePath;

                //save the author visual content to database
                var response = await _httpClientService.HttpClient.PostAsJsonAsync("authorvisualcontent", authorVisualContent);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return null;
                    }

                    authorVisualContent = await response.Content.ReadFromJsonAsync<AuthorVisualContentDTO>();
                    if (authorVisualContent is not null && authorVisualContent.Id != default)
                    {
                        bool fileSaved = await _fileService.SaveImageAsStream(localFilePath, authorVisualContent.FileName);
                        if (fileSaved)
                        {
                            return authorVisualContent;
                        }
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
                throw;
            }
        }

        private string GenerateRelativePath(AuthorVisualContentDTO authorVisualContent, string localFilePath)
        {

            string extension = Path.GetExtension(localFilePath);
            string newFileName = Path.GetRandomFileName();
            newFileName = Path.GetFileNameWithoutExtension(newFileName);
            string fileName = newFileName + extension;
            string relativePath = Path.Combine(_userDetailService.UserDetailModel.DisplayName!, authorVisualContent.VisualContentType, fileName);

            return relativePath;
        }

        public async Task<bool> DeleteAuthorVisualContent(int id)
        {
            try
            {
                HttpResponseMessage response = await _httpClientService.HttpClient.DeleteAsync($"AuthorVisualContent/{id}");

                if (response.IsSuccessStatusCode)
                {
                    bool deleted = await response.Content.ReadFromJsonAsync<bool>();
                    return deleted;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                if (ex.Message.Contains("Validation Error"))
                {
                    MessageBox.Show(ex.Message, "Delete Visual Content");
                    return false;
                }
                if (ex.Message.Contains("Not Allowed"))
                {
                    MessageBox.Show(ex.Message, "Delete Visual Content");
                    return false;
                }
                throw;
            }
        }

        public async Task<IEnumerable<AuthorVisualContentDTO>> GetAllAuthorVisualContent()
        {
            try
            {
                HttpResponseMessage response = await _httpClientService.HttpClient.GetAsync("AuthorVisualContent/getall");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<AuthorVisualContentDTO>();
                    }

                    IEnumerable<AuthorVisualContentDTO> authorVisualContents = await response.Content.ReadFromJsonAsync<IEnumerable<AuthorVisualContentDTO>>();
                    return authorVisualContents!;
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(message, "Retrieve Author Visual Content");
                    return Enumerable.Empty<AuthorVisualContentDTO>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public async Task<AuthorVisualContentDTO> GetAuthorVisualContent(int id)
        {
            try
            {
                HttpResponseMessage response = await _httpClientService.HttpClient.GetAsync($"AuthorVisualContent/{id}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return new AuthorVisualContentDTO();
                    }

                    AuthorVisualContentDTO authorVisualContent = await response.Content.ReadFromJsonAsync<AuthorVisualContentDTO>();
                    return authorVisualContent;
                }
                else
                {
                    return new AuthorVisualContentDTO();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public Task<bool> UpdateAuthorVisualContent(AuthorVisualContentDTO authorVisualContent, string localFilePath)
        {
            throw new NotImplementedException();
        }
    }
}
