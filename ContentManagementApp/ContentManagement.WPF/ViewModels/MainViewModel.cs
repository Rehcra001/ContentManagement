using ContentManagement.WPF.Core;
using ContentManagement.WPF.Services.Contracts;
using ContentManagement.WPF.ViewModels.Account;
using ContentManagement.WPF.ViewModels.Administration;
using Log = Serilog.Log;

namespace ContentManagement.WPF.ViewModels
{
    public class MainViewModel : ViewModel, IDisposable
    {
        public INavigationService NavigationService { get; set; }
        public IUserService UserService { get; set; }
        public IUserDetailService UserDetailService { get; set; }
        public IProcessJWTTokenService ProcessJWTTokenService { get; set; }

        private const string SHOW = "Visible";
        private const string HIDE = "Collapsed";


        private string _menuState = HIDE;
        public string MenuState
        {
            get { return _menuState; }
            set 
            {
                _menuState = value;
                OnPropertyChanged();
            }
        }

        private string _adminState = HIDE;
        public string AdminState
        {
            get { return _adminState; }
            set 
            { 
                _adminState = value;
                OnPropertyChanged();
            }
        }

        private string? _userName;
        public string? UserName
        {
            get { return _userName; }
            set 
            { 
                _userName = value;
                OnPropertyChanged();
            }
        }

        //commands
        public RelayCommand LogoutCommand { get; set; }
        public RelayCommand NavigateToAdminNewUserCommand { get; set; }
        public RelayCommand NavigateToEditUserCommand { get; set; }
        public RelayCommand NavigateToRemoveUserCommand { get; set; }
        public RelayCommand NavigateToUserDetailCommand { get; set; }
        public RelayCommand NavigateToChangePasswordCommand { get; set; }
        public RelayCommand NavigateToCategoryManagementCommand { get; set; }
        public RelayCommand NavigateToSubCategoryManagementCommand { get; set; }

        public MainViewModel(INavigationService navigationService,
                             IUserService userService,
                             IUserDetailService userDetailService,
                             IProcessJWTTokenService processJWTTokenService)
        {
            NavigationService = navigationService;
            UserService = userService;
            UserDetailService = userDetailService;
            ProcessJWTTokenService = processJWTTokenService;

            //Subscribe to events
            SubscribeEvents();

            //Commands
            LogoutCommand = new RelayCommand(Logout, CanLogout);
            NavigateToAdminNewUserCommand = new RelayCommand(NavigateToAdminNewUser, CanNavigateToAdminNewUser);
            NavigateToEditUserCommand = new RelayCommand(NavigateToEditUser, CanNavigateToEditUser);
            NavigateToRemoveUserCommand = new RelayCommand(NavigateToRemoveUser, CanNavigateToRemoveUser);
            NavigateToUserDetailCommand = new RelayCommand(NavigateToUserDetail, CanNavigateToUserDetail);
            NavigateToChangePasswordCommand = new RelayCommand(NavigateToChangePassword, CanNavigateToChangePassword);
            NavigateToCategoryManagementCommand = new RelayCommand(NavigateToCategoryManagement, CanNavigateToCategoryManagement);
            NavigateToSubCategoryManagementCommand = new RelayCommand(NavigateToSubCategoryManagement, CanNavigateToSubCategoryManagement);


            //Open log in page
            NavigationService.NavigateTo<LoginViewModel>();            
        }

        private bool CanNavigateToSubCategoryManagement(object obj)
        {
            return true;
        }

        private void NavigateToSubCategoryManagement(object obj)
        {
            NavigationService.NavigateTo<SubCategoryManagementViewModel>();
        }

        private bool CanNavigateToCategoryManagement(object obj)
        {
            return true;
        }

        private void NavigateToCategoryManagement(object obj)
        {
            NavigationService.NavigateTo<CategoryManagementViewModel>();
        }

        private bool CanNavigateToChangePassword(object obj)
        {
            return true;
        }

        private void NavigateToChangePassword(object obj)
        {
            NavigationService.NavigateTo<ChangePasswordViewModel>();
        }

        private bool CanNavigateToUserDetail(object obj)
        {
            return true;
        }

        private void NavigateToUserDetail(object obj)
        {
            NavigationService.NavigateTo<UserDetailViewModel>();
        }

        private bool CanNavigateToRemoveUser(object obj)
        {
            return true;
        }

        private void NavigateToRemoveUser(object obj)
        {
            NavigationService.NavigateTo<RemoveUserViewModel>();
        }

        private bool CanNavigateToEditUser(object obj)
        {
            return true;
        }

        private void NavigateToEditUser(object obj)
        {
            NavigationService.NavigateTo<EditUserViewModel>();
        }

        private bool CanNavigateToAdminNewUser(object obj)
        {
            string? role = UserDetailService.UserDetailModel.Role;
            if (role != null && role.Equals("Administrator"))
            {
                return true;
            }
            return false;
        }

        private void NavigateToAdminNewUser(object obj)
        {
            NavigationService.NavigateTo<NewUserViewModel>();
        }

        private bool CanLogout(object obj)
        {
            return true;
        }

        private void Logout(object obj)
        {
            UserService.RaiseEventOnLoggedInChanged(false);
            NavigationService.NavigateTo<LoginViewModel>();
        }

        private void SubscribeEvents()
        {
            //Subscribe to log in event
            UserService.OnLoggedInChanged += LoggedInChanged;
        }


        private void LoggedInChanged(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                MenuState = SHOW;
                UserName = UserDetailService.UserDetailModel.FirstName + " " + UserDetailService.UserDetailModel.LastName;
                SetAdministrator();
                Log.Information("{user} logged in.", UserDetailService.UserDetailModel.UserName);
                NavigationService.NavigateTo<HomeViewModel>();
            }
            else
            {
                MenuState = HIDE;
                UserName = "";
                UserDetailService.ClearUserDetails();
                ProcessJWTTokenService.ClearJwtToken();
                SetAdministrator();
            }
        }

        

        private void SetAdministrator()
        {
            if (!String.IsNullOrWhiteSpace(UserDetailService.UserDetailModel.Role)
                && UserDetailService.UserDetailModel.Role.Equals("Administrator"))
            {
                AdminState = SHOW;
            }
            else
            {
                AdminState = HIDE;
            }
        }

        public void Dispose()
        {
            UserService.OnLoggedInChanged -= LoggedInChanged;
        }
    }
}
