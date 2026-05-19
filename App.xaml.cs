using Lab11_Navigation.Interfaces;
using Lab11_Navigation.Repositories;
using Lab11_Navigation.Services;
using Lab11_Navigation.ViewModels;
using Lab11_Navigation.Views;
using Microsoft.EntityFrameworkCore;
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

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();

            // Регистрация DbContext
            services.AddDbContext<PhoneBookDbBabikov2307a1Context>(options =>
                options.UseSqlServer(
                    "Data Source=dbsrv\\gor2025;Initial Catalog=PhoneBookDB_Babikov_2307a1;Integrated Security=True;TrustServerCertificate=True"));

            services.AddScoped<IContactRepository, EfContactRepository>();

            services.AddTransient<AboutViewModel>();
            services.AddTransient<ContactsListViewModel>();
            services.AddTransient<ContactEditViewModel>();

            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<MainWindow>(sp =>
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
