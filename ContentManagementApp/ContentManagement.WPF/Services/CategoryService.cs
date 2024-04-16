using ContentManagement.DTOs;
using ContentManagement.WPF.Services.Contracts;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Log = Serilog.Log;

namespace ContentManagement.WPF.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IHttpClientService _httpClientService;

        public CategoryService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<CategoryDTO?> AddCategory(CategoryDTO? category)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.PostAsJsonAsync("category", category);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
                    {
                        return null;
                    }
                    category = await httpResponseMessage.Content.ReadFromJsonAsync<CategoryDTO>();
                    return category;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show("Error saving category", "Add New Category", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task<IEnumerable<CategoryDTO>?> GetCategories()
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.GetAsync("category");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<CategoryDTO>();
                    }
                    IEnumerable<CategoryDTO>? categories = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<CategoryDTO>>();
                    return categories;
                }
                return Enumerable.Empty<CategoryDTO>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show("Unable to retrieve the categories.", "Get Categories", MessageBoxButton.OK, MessageBoxImage.Error);
                return Enumerable.Empty<CategoryDTO>();
            }
        }

        public async Task<CategoryDTO?> GetCategory(int id)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.GetAsync($"category/{id}");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
                    {
                        return null;
                    }
                    CategoryDTO? category = await httpResponseMessage.Content.ReadFromJsonAsync<CategoryDTO>();
                    return category;
                }
                
                return null;

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show("Unable to retrieve the category.", "Get Category", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task<bool> RemoveCategory(int id)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.DeleteAsync($"category/{id}");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    //Means that the category is beign used in a post
                    string message = await httpResponseMessage.Content.ReadAsStringAsync();
                    MessageBox.Show(message, "Remove Category", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show("Unable to remove the category.", "Remove Category", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> UpdateCategory(CategoryDTO category)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.PutAsJsonAsync("category", category);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show("Unable to update the category.", "Update Category", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
