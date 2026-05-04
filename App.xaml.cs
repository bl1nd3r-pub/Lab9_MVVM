using Lab11_Navigation.Services;
using Lab11_Navigation.ViewModels;
using Lab11_Navigation.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Lab11_Navigation
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            var services = new ServiceCollection();

            services.AddSingleton<INavigationService, NavigationService>(); // Singleton: один экземпляр на всё приложение
            services.AddSingleton<IDialogService, DialogService>(); // Singleton: один экземпляр на всё приложение

            services.AddTransient<AboutViewModel>(); // Transient: новый экземпляр при каждой навигации
            services.AddTransient<ContactsListViewModel>();
            services.AddTransient<ContactEditViewModel>();

            services.AddSingleton<MainWindowViewModel>(); // Singleton

            services.AddSingleton<MainWindow>(sp => // Singleton: главное окно (Shell)
            {
                var window = new MainWindow();
                window.DataContext = sp.GetRequiredService<MainWindowViewModel>();
                return window;
            });

            var serviceProvider = services.BuildServiceProvider();

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
