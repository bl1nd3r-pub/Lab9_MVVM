using Lab11_Navigation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab11_Navigation.Services
{
    public class NavigationService :ObservableObject, INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private object? _currentViewModel;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public object? CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                _currentViewModel = value;
                OnPropertyChanged(); // Уведомляем UI, что экран сменился
            }
        }

        public void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class
        {
            // 1. Получаем экземпляр ViewModel из контейнера DI
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            // 2. Если ViewModel умеет принимать параметры, передаём их
            if (viewModel is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(parameter);
            }

            // 3. Меняем текущий экран. ContentControl в Shell подхватит это изменение
            //    и через DataTemplate отобразит нужный View.
            CurrentViewModel = viewModel;
        }
    }
}
