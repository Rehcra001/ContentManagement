using ContentManagement.DTOs;
using ContentManagement.WPF.Core;
using ContentManagement.WPF.Services.Contracts;
using ContentManagement.WPF.Validators;
using FluentValidation.Results;
using System.Windows;
using Log = Serilog.Log;

namespace ContentManagement.WPF.ViewModels.Account
{
    public class UserDetailViewModel : ViewModel
    { 
        public INavigationService NavigationService { get; set; }
        public IUserService UserService { get; set; }
        public IUserDetailService UserDetailService { get; set; }

        private const string VIEW = "View";
        private const string EDIT = "Edit";
        private const string SAVE = "Save";

        private string _viewState = VIEW;
        

        private UserDTO _user;
        public UserDTO User
        {
            get { return _user; }
            set { _user = value; OnPropertyChanged(); }
        }

        private bool _readOnly;
        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; OnPropertyChanged(); }
        }

        public RelayCommand SaveUserCommand { get; set; }
        public RelayCommand CancelUserCommand { get; set; }
        public RelayCommand EditUserCommand { get; set; }

        public UserDetailViewModel(INavigationService navigationService, IUserService userService, IUserDetailService userDetailService)
        {
            NavigationService = navigationService;
            UserService = userService;
            UserDetailService = userDetailService;

            LoadUserDetails();

            SetViewState(VIEW);

            SaveUserCommand = new RelayCommand(SaveUser, CanSaveUser);
            CancelUserCommand = new RelayCommand(CancelUser, CanCancelUser);
            EditUserCommand = new RelayCommand(EditUser, CanEditUser);
        }

        private bool CanEditUser(object obj)
        {
            return _viewState.Equals(VIEW);
        }

        private void EditUser(object obj)
        {
            SetViewState(EDIT);
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
            return _viewState.Equals(EDIT);
        }

        private async void SaveUser(object obj)
        {
            SetViewState(SAVE);
            //Check if any details were altered
            if (DetailsChanged())
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
                    //Save user details
                    bool saved = await UserService.UpdateUser(User);
                    //Update UserDetailService.UserDetailModel
                    if (saved)
                    {
                        UpdateUserDetails();
                        MessageBox.Show("Changes were successfully saved", "Save User Details", MessageBoxButton.OK, MessageBoxImage.Information);
                        SetViewState(VIEW);
                    }
                    else
                    {
                        MessageBox.Show("Changes were not saved.\r\nIf the problem persists please contact your administrator",
                                    "Save User Details",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                        SetViewState(EDIT);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                    MessageBox.Show("Changes were not saved.\r\nIf the problem persists please contact your administrator",
                                    "Save User Details",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    SetViewState(EDIT);
                    return;
                }
                
            }
            else
            {
                MessageBox.Show("No details were changed", "Change User Details", MessageBoxButton.OK, MessageBoxImage.Information);
                SetViewState(EDIT);
                return;
            }            
        }

        private void UpdateUserDetails()
        {
            UserDetailService.UserDetailModel.FirstName = User.FirstName;
            UserDetailService.UserDetailModel.LastName = User.LastName;
            UserDetailService.UserDetailModel.DisplayName = User.DisplayName;
        }

        private string? Validate()
        {
            var validator = new UserValidator();
            ValidationResult validationResult = validator.Validate(User);

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

        private void SetViewState(string viewState)
        {
            _viewState = viewState;

            switch (_viewState)
            {
                case VIEW:
                    ReadOnly = true;
                    break;
                case EDIT:
                    ReadOnly = false;
                    break;
                case SAVE:
                    ReadOnly = true;
                    break;
            }
        }

        private bool DetailsChanged()
        {
            if (!User.FirstName.Equals(UserDetailService.UserDetailModel.FirstName))
            {
                return true;
            }
            if (!User.LastName.Equals(UserDetailService.UserDetailModel.LastName))
            {
                return true;
            }
            if (!User.DisplayName.Equals(UserDetailService.UserDetailModel.DisplayName))
            {
                return true;
            }

            return false;
        }

        private void LoadUserDetails()
        {
            User = new UserDTO
            {
                EmailAddress = UserDetailService.UserDetailModel.EmailAddress,
                FirstName = UserDetailService.UserDetailModel.FirstName,
                LastName = UserDetailService.UserDetailModel.LastName,
                DisplayName = UserDetailService.UserDetailModel.DisplayName,
                Role = UserDetailService.UserDetailModel.Role
            };
        }
    }
}
