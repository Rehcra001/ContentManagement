using ContentManagement.DTOs;
using ContentManagement.WPF.Core;
using ContentManagement.WPF.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.WPF.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public INavigationService NavigationService { get; set; }
        public IUserService UserService { get; set; }

        
        public MainViewModel(INavigationService navigationService,IUserService userService)
        {
            NavigationService = navigationService;
            UserService = userService;

            //Open log in page
            NavigationService.NavigateTo<LoginViewModel>();            
        }        
    }
}
