using ContentManagement.WPF.Models;
using ContentManagement.WPF.Services.Contracts;

namespace ContentManagement.WPF.Services
{
    public class UserDetailService : IUserDetailService
    {
        private UserDetailModel _userDetailModel = new UserDetailModel();
        public UserDetailModel UserDetailModel { get => _userDetailModel; set => _userDetailModel = value; }
    }
}
