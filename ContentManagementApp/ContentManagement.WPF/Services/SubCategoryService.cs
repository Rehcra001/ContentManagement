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
    public class SubCategoryService : ISubCategoryService
    {
        private readonly IHttpClientService _httpClientService;

        public SubCategoryService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<SubCategoryDTO?> AddSubCategory(SubCategoryDTO? subCategory)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.PostAsJsonAsync("subcategory", subCategory);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
                    {
                        return null;
                    }
                    subCategory = await httpResponseMessage.Content.ReadFromJsonAsync<SubCategoryDTO>();
                    return subCategory;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show("Error saving sub category", "Add New Sub Category", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task<IEnumerable<SubCategoryDTO>?> GetSubCategories()
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.GetAsync("subcategory");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<SubCategoryDTO>();
                    }
                    IEnumerable<SubCategoryDTO>? subCategories = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<SubCategoryDTO>>();
                    return subCategories;
                }
                return Enumerable.Empty<SubCategoryDTO>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show("Unable to retrieve the sub categories.", "Get Sub Categories", MessageBoxButton.OK, MessageBoxImage.Error);
                return Enumerable.Empty<SubCategoryDTO>();
            }
        }

        public async Task<SubCategoryDTO?> GetSubCategory(int id)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.GetAsync($"subcategory/{id}");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
                    {
                        return null;
                    }
                    SubCategoryDTO? subCategory = await httpResponseMessage.Content.ReadFromJsonAsync<SubCategoryDTO>();
                    return subCategory;
                }

                return null;

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show("Unable to retrieve the sub category.", "Get Sub Category", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task<bool> RemoveSubCategory(int id)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.DeleteAsync($"subcategory/{id}");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    //Means that the category is beign used in a post
                    string message = await httpResponseMessage.Content.ReadAsStringAsync();
                    MessageBox.Show(message, "Remove sub Category", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show("Unable to remove the sub category.", "Remove Sub Category", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> UpdateSubCategory(SubCategoryDTO subCategory)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.PutAsJsonAsync("subcategory", subCategory);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show("Unable to update the sub category.", "Update Sub Category", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
