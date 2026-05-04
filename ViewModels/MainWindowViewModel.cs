using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Lab11_Navigation.Services;

namespace Lab11_Navigation.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        public INavigationService NavigationService => _navigationService;

        public ICommand ShowContactsCommand { get; }
        public ICommand ShowAboutCommand { get; }

        public MainWindowViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            // Команды для переключения экранов
            ShowContactsCommand = new RelayCommand(() =>
                _navigationService.NavigateTo<ContactsListViewModel>());

            ShowAboutCommand = new RelayCommand(() =>
                _navigationService.NavigateTo<AboutViewModel>());

            // При запуске сразу показываем список контактов
            _navigationService.NavigateTo<ContactsListViewModel>();
        }
    }
}
