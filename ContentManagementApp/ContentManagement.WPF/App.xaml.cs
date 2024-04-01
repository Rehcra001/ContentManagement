using ContentManagement.WPF.Core;
using ContentManagement.WPF.Services;
using ContentManagement.WPF.Services.Contracts;
using ContentManagement.WPF.ViewModels;
using ContentManagement.WPF.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;

namespace ContentManagement.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        /// <summary>
        /// Adds access to appsettings.json
        /// </summary>
        /// <returns>
        /// Returns an IConfiguration
        /// </returns>
        private IConfiguration AddConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            return builder.Build();
        }

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            
            //register views
            services.AddSingleton<MainView>(provider => new MainView
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            services.AddSingleton<MainViewModel>();
            services.AddTransient<LoginViewModel>();

            //Add appsettings.json Configuration
            services.AddSingleton(AddConfiguration());


            //Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IHttpClientService, HttpClientService>();
            services.AddSingleton<IUserDetailService, UserDetailService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IProcessJWTTokenService, ProcessJWTTokenService>();

            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainView>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }

}
