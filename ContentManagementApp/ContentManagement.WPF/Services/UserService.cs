using ContentManagement.DTOs;
using ContentManagement.WPF.Services.Contracts;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
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

        public async Task<bool> RegisterNewUser(UserRegistrationDTO userRegistrationDTO)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.PostAsJsonAsync<UserRegistrationDTO>("user/register", userRegistrationDTO);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return true;
                }
                var message = await httpResponseMessage.Content.ReadAsStringAsync();
                Log.Error(message);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public Task<UserDTO> GetUser()
        {
            throw new NotImplementedException();
        }

        public Task<UserDTO> GetUser(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserDTO>> GetUsers()
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _httpClientService.HttpClient.GetAsync("user/users");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<UserDTO>();
                    }
                    else
                    {
                        IEnumerable<UserDTO>? users = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();
                        return users!;
                    }                    
                }
                else
                {
                    var message = await httpResponseMessage.Content.ReadAsStringAsync();
                    Log.Error($"Http status: {httpResponseMessage.StatusCode} Message -{message}");
                    return Enumerable.Empty<UserDTO>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public Task<bool> RemoveUser(string email)
        {
            throw new NotImplementedException();
        }

        public Task<UserDTO> UpdateUser(string email)
        {
            throw new NotImplementedException();
        }
    }
}
