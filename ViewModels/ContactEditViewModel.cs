using Lab11_Navigation.Interfaces;
using Lab11_Navigation.Models;
using Lab11_Navigation.Repositories;
using Lab11_Navigation.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lab11_Navigation.ViewModels
{
    public class ContactEditViewModel : ObservableObject, INavigationAware
    {
        private readonly IContactRepository _repository;
        private readonly INavigationService _navigation;
        private readonly IDialogService _dialogService;
        private Contact _originalContact = null!;
        private Contact _editingContact = null!;

        public string EditName
        {
            get => _editingContact.Name;
            set
            {
                _editingContact.Name = value;
                OnPropertyChanged();
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public string EditPhone
        {
            get => _editingContact.Phone;
            set
            {
                _editingContact.Phone = value;
                OnPropertyChanged();
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ContactEditViewModel(
            IContactRepository repository,
            INavigationService navigation,
            IDialogService dialogService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(async () => await SaveAsync());
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSave() => _editingContact?.Validate() == true;

        private async Task SaveAsync()
        {
            if (_editingContact == null || !_editingContact.Validate())
                return;

            // Проверка на дубликат (исключая текущий контакт)
            if (await _repository.ExistsByPhoneAsync(EditPhone, excludeId: _editingContact.Id))
            {
                _dialogService.ShowWarning("Контакт с таким номером уже существует!");
                return;
            }

            try
            {
                await _repository.UpdateAsync(_editingContact);
                _dialogService.ShowInfo("Изменения сохранены");
                _navigation.NavigateTo<ContactsListViewModel>();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void Cancel()
        {
            // Просто возвращаемся назад без сохранения
            _navigation.NavigateTo<ContactsListViewModel>();
        }

        public void OnNavigatedTo(object? parameter)
        {
            if (parameter is Contact contact)
            {
                // Сохраняем оригинал и создаём редактируемую копию
                _originalContact = contact;
                _editingContact = new Contact
                {
                    Id = contact.Id,
                    Name = contact.Name,
                    Phone = contact.Phone
                };

                OnPropertyChanged(nameof(EditName));
                OnPropertyChanged(nameof(EditPhone));
            }
            else
            {
                _navigation.NavigateTo<ContactsListViewModel>();
            }
        }
    }
}