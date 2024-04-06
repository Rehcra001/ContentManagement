using ContentManagement.DTOs;
using ContentManagement.WPF.Core;
using ContentManagement.WPF.Services.Contracts;
using ContentManagement.WPF.Validators;
using FluentValidation.Results;
using System.Collections.ObjectModel;
using System.Windows;
using Log = Serilog.Log;
using ContentManagement.WPF;
using ContentManagement.WPF.Enums;

namespace ContentManagement.WPF.ViewModels.Administration
{
    public class NewUserViewModel : ViewModel
    {
        public INavigationService NavigationService { get; set; }
        public IUserService UserService { get; set; }


        private UserRegistrationDTO _newUser = new UserRegistrationDTO();
        public UserRegistrationDTO NewUser
        {
            get { return _newUser; }
            set { _newUser = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _roles;

        public ObservableCollection<string> Roles
        {
            get { return _roles; }
            set { _roles = value; OnPropertyChanged(); }
        }

        private string _selectedRole;

        public string SelectedRole
        {
            get { return _selectedRole; }
            set { _selectedRole = value; OnPropertyChanged(); }
        }


        public RelayCommand SaveUserCommand { get; set; }
        public RelayCommand CancelUserCommand { get; set; }

        public NewUserViewModel(INavigationService navigationService, IUserService userService)
        {
            NavigationService = navigationService;
            UserService = userService;

            //add roles
            Roles = new ObservableCollection<string>();
            var roleValues = Enum.GetValues(typeof(Roles));
            foreach (var role in roleValues)
            {
                Roles.Add(role.ToString()!);
            }

            SaveUserCommand = new RelayCommand(SaveUser, CanSaveUser);
            CancelUserCommand = new RelayCommand(CancelUser, CanCancelUser);
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
                bool succeeded = await UserService.RegisterNewUser(NewUser);
                if (succeeded)
                {
                    MessageBox.Show("New user has been saved!", "Save User", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.NavigateTo<HomeViewModel>();
                }
                else
                {
                    MessageBox.Show("Unexpected Erorr: Please try again.\r\nIf this error persists please let your administrator know", "Save User", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        private string? Validate()
        {
            var validator = new NewUserValidator();
            ValidationResult validationResult = validator.Validate(NewUser);

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

        private bool CheckFields()
        {
            //All properties required
            foreach (var property in NewUser.GetType().GetProperties())
            {
                if (String.IsNullOrWhiteSpace(property.GetValue(NewUser)?.ToString()))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
