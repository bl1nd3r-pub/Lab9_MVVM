using Lab11_Navigation.Models;
using Lab11_Navigation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lab11_Navigation.ViewModels
{
    public class ContactEditViewModel : ObservableObject, INavigationAware
    {
        private readonly INavigationService _navigation;
        private Contact _contact = null!; // Будет инициализирован в OnNavigatedTo

        public string EditName
        {
            get => _contact.Name;
            set
            {
                _contact.Name = value;
                OnPropertyChanged();
            }
        }

        public string EditPhone
        {
            get => _contact.Phone;
            set
            {
                _contact.Phone = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ContactEditViewModel(INavigationService navigation)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

            // Обе команды просто возвращают нас на список контактов
            // В реальной работе SaveCommand мог бы вызывать сервис сохранения данных
            SaveCommand = new RelayCommand(() =>
                _navigation.NavigateTo<ContactsListViewModel>());

            CancelCommand = new RelayCommand(() =>
                _navigation.NavigateTo<ContactsListViewModel>());
        }

        // Этот метод вызывается автоматически сервисом навигации при переходе на этот экран
        public void OnNavigatedTo(object? parameter)
        {
            if (parameter is Contact contact)
            {
                _contact = contact;
            }
            else
            {
                // Если параметр не передан (ошибка логики), возвращаемся назад
                _navigation.NavigateTo<ContactsListViewModel>();
            }
        }
    }
}
