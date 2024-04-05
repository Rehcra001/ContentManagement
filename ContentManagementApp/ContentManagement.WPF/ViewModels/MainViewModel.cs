using ContentManagement.WPF.Core;
using ContentManagement.WPF.Services.Contracts;

namespace ContentManagement.WPF.ViewModels
{
    public class MainViewModel : ViewModel, IDisposable
    {
        public INavigationService NavigationService { get; set; }
        public IUserService UserService { get; set; }
        public IUserDetailService UserDetailService { get; set; }

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

        private string _adminState;
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

        public MainViewModel(INavigationService navigationService,
                             IUserService userService,
                             IUserDetailService userDetailService)
        {
            NavigationService = navigationService;
            UserService = userService;
            UserDetailService = userDetailService;

            //Subscribe to events
            SubscribeEvents();

            //Commands
            LogoutCommand = new RelayCommand(Logout, CanLogout);

            //Open log in page
            NavigationService.NavigateTo<LoginViewModel>();            
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
                NavigationService.NavigateTo<HomeViewModel>();
            }
            else
            {
                MenuState = HIDE;
                UserName = "";
                UserDetailService.ClearUserDetails();
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
