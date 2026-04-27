using Lab10_DI.Services;
using Lab10_DI.ViewModels;
using Lab10_DI.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Lab10_DI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Созданиек коллекции для регистрации сервисов
            var services = new ServiceCollection(); // Великий Майковский сервис колекшн

            // 2. Регистрация сервисов сервисы
            services.AddSingleton<IDialogService, DialogService>(); // Singleton: один экземпляр на всё приложение
            services.AddTransient<MainViewModel>(); // Transient: новый экземпляр при каждом запросе
            services.AddSingleton<MainWindow>(sp => // Singleton + явная настройка DataContext
            {
                var window = new MainWindow();
                window.DataContext = sp.GetRequiredService<MainViewModel>();
                return window;
            });

            // 3. Контейнер (ServiceProvider)
            var serviceProvider = services.BuildServiceProvider();

            // 4. Получение и отображение главного окна
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
