﻿using ContentManagement.DTOs;
using ContentManagement.WPF.Services.Contracts;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Log = Serilog.Log;


namespace ContentManagement.WPF.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUserDetailService _userDetailService;
        private readonly IProcessJWTTokenService _processJWTTokenService;

        public event Action<bool> OnLoggedInChanged;

        public UserService(IHttpClientService httpClientService,
                           IUserDetailService userDetailService,
                           IProcessJWTTokenService processJWTTokenService)
        {
            _httpClientService = httpClientService;
            _userDetailService = userDetailService;
            _processJWTTokenService = processJWTTokenService;
        }

        

        public Task ChangeUserPassword(ChangePasswordDTO changePasswordDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> LoginUser(UserSignInDTO userSignInDTO)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.PostAsJsonAsync<UserSignInDTO>("user/signin", userSignInDTO);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var token = await httpResponseMessage.Content.ReadAsStringAsync();
                    _processJWTTokenService.ProcessJwtToken(token);
                    return true;
                }
                else
                {
                    var message = await httpResponseMessage.Content.ReadAsStringAsync();
                    Log.Error($"Http status: {httpResponseMessage.StatusCode} Message -{message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // TODO - Log exception
                Log.Error("Login Error: " + ex.Message);
                throw;
            }
        }

        public void RaiseEventOnLoggedInChanged(bool isLoggedIn)
        {
                //Has subscribers
                OnLoggedInChanged?.Invoke(isLoggedIn);
        }

        public Task<bool> RegisterNewUser(UserRegistrationDTO userRegistrationDTO)
        {
            throw new NotImplementedException();
        }
    }
}
