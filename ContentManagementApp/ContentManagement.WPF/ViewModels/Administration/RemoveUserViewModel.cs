using ContentManagement.DTOs;
using ContentManagement.WPF.Core;
using ContentManagement.WPF.Services.Contracts;
using System.Collections.ObjectModel;
using System.Windows;
using Log = Serilog.Log;

namespace ContentManagement.WPF.ViewModels.Administration
{
    public class RemoveUserViewModel : ViewModel
    {
        public INavigationService NavigationService { get; set; }
        public IUserService UserService { get; set; }

        private UserDTO? _selectedUser;
        public UserDTO? SelectedUser
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

        public RelayCommand RemoveUserCommand { get; set; }
        public RelayCommand CancelUserCommand { get; set; }

        public RemoveUserViewModel(INavigationService navigationService,
                                   IUserService userService)
        {
            NavigationService = navigationService;
            UserService = userService;

            RemoveUserCommand = new RelayCommand(RemoveUser, CanRemoveUser);
            CancelUserCommand = new RelayCommand(CancelUser, CanCancelUser);

            GetUsers();

        }

        private bool CanCancelUser(object obj)
        {
            return true;
        }

        private void CancelUser(object obj)
        {
            NavigationService.NavigateTo<HomeViewModel>();
        }

        private bool CanRemoveUser(object obj)
        {
            return SelectedUser != null;
        }

        private async void RemoveUser(object obj)
        {
            try
            {
                if (Users != null && Users.Count > 0)
                {
                    bool removed = await UserService.RemoveUser(SelectedUser.EmailAddress!);

                    if (removed)
                    {

                        int index = 0;
                        for (index = 0; index < Users.Count(); index++)
                        {
                            if (Users[index].EmailAddress.Equals(SelectedUser.EmailAddress))
                            {
                                break;
                            }
                        }
                        Users.RemoveAt(index);
                        SelectedUser = null; //Set to first in list
                        MessageBox.Show("User removed!", "Update User");

                    }
                    else
                    {
                        MessageBox.Show("User was not remove!\r\nPlease try again. If this error persists please contact your administrator",
                                        "Remove User",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }              
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
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
