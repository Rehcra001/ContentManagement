using ContentManagement.DTOs;
using ContentManagement.WPF.Core;
using ContentManagement.WPF.Enums;
using ContentManagement.WPF.Services.Contracts;
using ContentManagement.WPF.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Log = Serilog.Log;
using FluentValidation.Results;

namespace ContentManagement.WPF.ViewModels.Administration
{
    public class EditUserViewModel : ViewModel
    {
        public INavigationService NavigationService { get; set; }
        public IUserService UserService { get; set; }

        private UserDTO _selectedUser;
        public UserDTO SelectedUser
        {
            get { return _selectedUser; }
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        private ObservableCollection<UserDTO>? _users;
        public ObservableCollection<UserDTO>? Users
        {
            get { return _users; }
            set { _users = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _roles;

        public ObservableCollection<string> Roles
        {
            get { return _roles; }
            set { _roles = value; OnPropertyChanged(); }
        }

        public RelayCommand SaveUserCommand { get; set; }
        public RelayCommand CancelUserCommand { get; set; }

        public EditUserViewModel(INavigationService navigationService, IUserService userService)
        {
            NavigationService = navigationService;
            UserService = userService;

            SaveUserCommand = new RelayCommand(SaveUser, CanSaveUser);
            CancelUserCommand = new RelayCommand(CancelUser, CanCancelUser);

            GetUsers();

            AddRoles();

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
            return SelectedUser != null;
        }

        private async void SaveUser(object obj)
        {
            //Validate user
            string? validationResults = Validate();
            if (validationResults != null)
            {
                MessageBox.Show(validationResults, "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                bool succeeded = await UserService.UpdateUser(SelectedUser);

                if (succeeded)
                {
                    MessageBox.Show("User updated!", "Update User");
                }
                else
                {
                    MessageBox.Show("User was not updated!\r\nPlease try again. If this error persists please contact your administrator", "Update User", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var validator = new UserValidator();
            ValidationResult validationResult = validator.Validate(SelectedUser);

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

        private void AddRoles()
        {
            //add roles
            Roles = new ObservableCollection<string>();
            var roleValues = Enum.GetValues(typeof(Roles));
            foreach (var role in roleValues)
            {
                Roles.Add(role.ToString()!);
            }
        }

        private async void GetUsers()
        {
            Users = new ObservableCollection<UserDTO>();
            var users = await UserService.GetUsers();
            users.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
            foreach (var user in users)
            {
                Users.Add(user);
            }

        }
    }
}
