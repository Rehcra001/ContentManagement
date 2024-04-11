using ContentManagement.DTOs;
using ContentManagement.WPF.Core;
using ContentManagement.WPF.Services.Contracts;
using ContentManagement.WPF.Validators;
using FluentValidation.Results;
using System.Windows;
using Log = Serilog.Log;

namespace ContentManagement.WPF.ViewModels.Account
{
    public class ChangePasswordViewModel : ViewModel
    {      
        public INavigationService NavigationService { get; set; }
        public IUserService UserService { get; set; }

        private ChangePasswordDTO _changePassword;

        public ChangePasswordDTO ChangePassword
        {
            get { return _changePassword; }
            set { _changePassword = value; OnPropertyChanged(); }
        }

        public RelayCommand SaveUserCommand { get; set; }
        public RelayCommand CancelUserCommand { get; set; }

        public ChangePasswordViewModel(INavigationService navigationService, IUserService userService)
        {
            NavigationService = navigationService;
            UserService = userService;

            SaveUserCommand = new RelayCommand(SaveUser, CanSaveUser);
            CancelUserCommand = new RelayCommand(CancelUser, CanCancelUser);

            ChangePassword = new ChangePasswordDTO();
        }

        private bool CanCancelUser(object obj)
        {
            return true;
        }

        private void CancelUser(object obj)
        {
            NavigationService.NavigateTo<HomeViewModel>();
        }

        private bool CanSaveUser(object obj)
        {
            //return CheckFields();
            return true;
        }

        private async void SaveUser(object obj)
        {
            //Validate new user
            string? validationResults = Validate();
            if (validationResults != null)
            {
                MessageBox.Show(validationResults, "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                bool succeeded = await UserService.ChangeUserPassword(ChangePassword);  
                if (succeeded)
                {
                    NavigationService.NavigateTo<HomeViewModel>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show(ex.Message, "Change Password Errors", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string? Validate()
        {
            var validator = new ChangePasswordValidator();
            ValidationResult validationResult = validator.Validate(ChangePassword);

            string errors = "";
            if (validationResult.IsValid == false)
            {
                foreach (var failure in validationResult.Errors)
                {
                    errors += $"{failure.ErrorMessage}\r\n";
                }
                return errors;
            }
            return null;
        }
    }
}
