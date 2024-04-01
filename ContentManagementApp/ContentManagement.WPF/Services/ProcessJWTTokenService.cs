using ContentManagement.WPF.Services.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ContentManagement.WPF.Services
{
    public class ProcessJWTTokenService : IProcessJWTTokenService
    {
        private readonly IHttpClientService _httpClient;
        private readonly IUserDetailService _userDetail;

        public ProcessJWTTokenService(IHttpClientService httpClient, IUserDetailService userDetail)
        {
            _httpClient = httpClient;
            _userDetail = userDetail;
        }

        public void ProcessJwtToken(string token)
        {
            // remove quotes
            token = token.Replace('"', ' ').Trim();

            // add to header
            _httpClient.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Add claims to user detail
            ProcessClaims(token);
        }

        private void ProcessClaims(string token)
        {
            //add to user detail
            var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            IEnumerable<Claim> claims = jwtSecurityToken.Claims;

            _userDetail.UserDetailModel.EmailAddress = claims.First(c => c.Type == "sub").Value;
            _userDetail.UserDetailModel.UserName = claims.First(c => c.Type == "sub").Value;
            _userDetail.UserDetailModel.FirstName = claims.First(c => c.Type == "given_name").Value;
            _userDetail.UserDetailModel.LastName = claims.First(c => c.Type == "family_name").Value;
            _userDetail.UserDetailModel.DisplayName = claims.First(c => c.Type == "DisplayName").Value;
        }
    }
}
