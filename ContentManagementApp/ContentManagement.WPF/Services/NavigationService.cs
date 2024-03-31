using ContentManagement.WPF.Core;
using ContentManagement.WPF.Services.Contracts;

namespace ContentManagement.WPF.Services
{
    public class NavigationService : ObservableObject, INavigationService
    {
        
        private ViewModel _currentView;
        private readonly Func<Type, ViewModel> _viewModelFactory;

        public ViewModel CurrentView
        {
            get
            {
                return _currentView;
            }
            private set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public NavigationService(Func<Type, ViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModel
        {
            var viewMdoel = _viewModelFactory.Invoke(typeof(TViewModel));
            CurrentView = viewMdoel;
        }
    }
}
