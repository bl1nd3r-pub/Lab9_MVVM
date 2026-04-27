using Lab10_DI.Models;
using Lab10_DI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lab10_DI.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        // Коллекция контактов
        public ObservableCollection<Contact> Contacts { get; }
        
        private readonly IDialogService _dialogService;

        private string _name = string.Empty;
        private string _phone = string.Empty;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
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
        // Команды
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public MainViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            Contacts = new ObservableCollection<Contact>();
            AddCommand = new RelayCommand(AddContact, () => CanAddContact());
            DeleteCommand = new RelayCommand<object?>(DeleteContact, CanDeleteContact);
        }
        private void AddContact()
        {
            if (Contacts.Any(c => c.Phone == Phone))
            {
                _dialogService.ShowWarning("Контакт с таким номером уже существует!");
                return;
            }

            Contact newCont = new Contact(Name, Phone);
            Contacts.Add(newCont);
            Name = string.Empty;
            Phone = string.Empty;

            _dialogService.ShowInfo("Контакт был успешно добавлен");
        }
        private bool CanAddContact()
        {
            return (!string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Phone));
        }
        private void DeleteContact(object? param)
        {
            if (SelectedContact != null) {
                bool confirmed = _dialogService.ShowConfirmation( $"Удалить контакт \"{SelectedContact.Name}\"?", "Подтверждение");
                if (confirmed) {Contacts.Remove(SelectedContact);}
            }
        }
        private bool CanDeleteContact(object? param)
        {
            return (SelectedContact != null) ;
        }
    }
}
