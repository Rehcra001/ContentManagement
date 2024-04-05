using ContentManagement.DTOs;
using ContentManagement.WPF.Core;
using ContentManagement.WPF.Services.Contracts;
using ContentManagement.WPF.Validators;
using System.Windows;
using Log = Serilog.Log;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace ContentManagement.WPF.ViewModels
{
    public class LoginViewModel : ViewModel
    {
        public INavigationService NavigationService { get; set; }
        public IUserService UserService { get; set; }


        private UserSignInDTO _userSignIn = new UserSignInDTO();
        public UserSignInDTO UserSignIn
        {
            get { return _userSignIn; }
            set 
            { 
                _userSignIn = value;
                OnPropertyChanged();
            }
        }


        public RelayCommand LoginCommand { get; set; }

        public LoginViewModel(INavigationService navigationService, IUserService userService)
        {
            NavigationService = navigationService;
            UserService = userService;

            //Set logged
            UserService.RaiseEventOnLoggedInChanged(false);

            LoginCommand = new RelayCommand(Login, CanLogin);
        }

        private bool CanLogin(object obj)
        {
            return true;
        }

        private async void Login(object obj)
        {
            try
            {
                // Validate DTO
                var validator = new UserSignInValidator();
                ValidationResult validationResult = validator.Validate(UserSignIn);

                if (validationResult.IsValid == false)
                {
                    string errors = "";
                    foreach (var failure in validationResult.Errors)
                    {
                        errors += $"{failure.ErrorMessage}\r\n";
                    }
                    MessageBox.Show(errors, "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                bool loginSuccess = await UserService.LoginUser(UserSignIn);

                if (loginSuccess)
                {
                    Log.Information("Logged into WPF");
                    UserService.RaiseEventOnLoggedInChanged(true);
                    //NavigationService.NavigateTo<HomeViewModel>
                }
                else
                {
                    UserService.RaiseEventOnLoggedInChanged(false);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show("Log in Error: Please try again");
            }
        }
    }
}
