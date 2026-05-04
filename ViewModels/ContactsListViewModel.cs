using Lab11_Navigation.Models;
using Lab11_Navigation.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lab11_Navigation.ViewModels
{
    public class ContactsListViewModel : ObservableObject
    {
        private readonly INavigationService _navigation;
        private readonly IDialogService _dialogService;

        public ObservableCollection<Contact> Contacts { get; }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _phone = string.Empty;
        public string Phone
        {
            get => _phone;
            set => Set(ref _phone, value);
        }

        private Contact? _selectedContact;
        public Contact? SelectedContact
        {
            get => _selectedContact;
            set => Set(ref _selectedContact, value);
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditContactCommand { get; }

        public ContactsListViewModel(INavigationService navigation, IDialogService dialogService)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            Contacts = new ObservableCollection<Contact>();

            AddCommand = new RelayCommand(AddContact, CanAddContact);
            DeleteCommand = new RelayCommand<object?>(DeleteContact, CanDeleteContact);

            EditContactCommand = new RelayCommand<object?>(EditContact);
        }

        private void AddContact()
        {
            if (Contacts.Any(c => c.Phone == Phone))
            {
                _dialogService.ShowWarning("Контакт с таким номером уже существует!");
                return;
            }

            var contact = new Contact(Name, Phone);
            Contacts.Add(contact);
            Name = string.Empty;
            Phone = string.Empty;
            _dialogService.ShowInfo("Контакт успешно добавлен");
        }

        private bool CanAddContact() => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Phone);

        private void DeleteContact(object? param)
        {
            if (SelectedContact != null)
            {
                bool confirmed = _dialogService.ShowConfirmation($"Удалить контакт \"{SelectedContact.Name}\"?", "Подтверждение");
                if (confirmed)
                {
                    Contacts.Remove(SelectedContact);
                }
            }
        }

        private bool CanDeleteContact(object? param) => SelectedContact != null;

        // Метод для навигации на экран редактирования
        private void EditContact(object? parameter)
        {
            System.Diagnostics.Debug.WriteLine($"EditContact вызван. Parameter: {parameter?.GetType().Name}");

            if (parameter is Contact contact)
            {
                System.Diagnostics.Debug.WriteLine($"Переход к редактированию: {contact.Name}");
                _navigation.NavigateTo<ContactEditViewModel>(contact);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Ошибка: параметр не является контактом или null!");
            }
        }
    }
}
