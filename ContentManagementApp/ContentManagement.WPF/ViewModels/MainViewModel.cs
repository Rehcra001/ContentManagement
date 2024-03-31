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

        //public RelayCommand NavigateLoginCommand { get; set; }
        public MainViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;

            NavigationService.NavigateTo<LoginViewModel>();

            //NavigateLoginCommand = new RelayCommand(o => { NavigationService.NavigateTo<LoginViewModel>(); }, o => true);
        }
    }
}
