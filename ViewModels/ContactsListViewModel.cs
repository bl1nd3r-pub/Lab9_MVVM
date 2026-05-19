// ContactsListViewModel.cs
using Lab11_Navigation.Interfaces;
using Lab11_Navigation.Models;
using Lab11_Navigation.Repositories;
using Lab11_Navigation.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Lab11_Navigation.ViewModels
{
    public class ContactsListViewModel : ObservableObject
    {
        private readonly IContactRepository _repository;
        private readonly INavigationService _navigation;
        private readonly IDialogService _dialogService;
        private bool _isInitialized;

        // Коллекция для привязки к UI
        public ObservableCollection<Contact> Contacts { get; } = new();

        // Свойства для формы добавления/редактирования
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

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        // Команды
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand RefreshCommand { get; }

        public ContactsListViewModel(
            IContactRepository repository,
            INavigationService navigation,
            IDialogService dialogService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Инициализация команд
            AddCommand = new RelayCommand(async () => { await AddContactAsync(); }, CanAddContact);
            DeleteCommand = new RelayCommand(async () => { await DeleteContactAsync(); }, CanDeleteContact);
            EditCommand = new RelayCommand<Contact?>(EditContact, c => c != null);
            RefreshCommand = new RelayCommand(async () => { await LoadContactsAsync(); });

            // Автоматическая загрузка при создании
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            if (_isInitialized) return;

            IsLoading = true;
            try
            {
                await LoadContactsAsync();
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка инициализации: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadContactsAsync()
        {
            IsLoading = true;
            try
            {
                var contacts = await _repository.GetAllAsync();

                Contacts.Clear();
                foreach (var contact in contacts)
                    Contacts.Add(contact);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Не удалось загрузить контакты: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddContactAsync()
        {
            if (!CanAddContact()) return;

            // Проверка на дубликат по телефону
            if (await _repository.ExistsByPhoneAsync(Phone.Trim()))
            {
                _dialogService.ShowWarning("Контакт с таким номером уже существует!");
                return;
            }

            var contact = new Contact
            {
                Name = Name.Trim(),
                Phone = Phone.Trim()
            };

            if (!contact.Validate())
            {
                _dialogService.ShowWarning("Проверьте корректность введённых данных");
                return;
            }

            try
            {
                await _repository.AddAsync(contact);

                // Добавляем в коллекцию для мгновенного отображения в UI
                Contacts.Add(contact);

                // Сброс формы
                Name = string.Empty;
                Phone = string.Empty;


                _dialogService.ShowInfo("Контакт успешно добавлен");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка при добавлении: {ex.Message}");
            }
        }

        private bool CanAddContact() =>
            !string.IsNullOrWhiteSpace(Name) &&
            !string.IsNullOrWhiteSpace(Phone) &&
            !_isLoading;

        private async Task DeleteContactAsync()
        {
            if (SelectedContact == null) return;

            bool confirmed = _dialogService.ShowConfirmation(
                $"Удалить контакт \"{SelectedContact.Name}\"?",
                "Подтверждение удаления");

            if (!confirmed) return;

            try
            {
                await _repository.DeleteAsync(SelectedContact.Id);
                Contacts.Remove(SelectedContact);
                SelectedContact = null;

                _dialogService.ShowInfo("Контакт удалён");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка при удалении: {ex.Message}");
            }
        }

        private bool CanDeleteContact() => SelectedContact != null && !_isLoading;

        private void EditContact(Contact? contact)
        {
            if (contact == null) return;

            // Навигация на экран редактирования с передачей контакта
            // Убедись, что ContactEditViewModel принимает Contact в конструкторе или через параметр
            _navigation.NavigateTo<ContactEditViewModel>(contact);
        }

        private async Task SaveContactAsync(Contact? contact)
        {
            if (contact == null || !contact.Validate()) return;

            // Проверка на дубликат (исключаем текущий контакт при редактировании)
            if (await _repository.ExistsByPhoneAsync(contact.Phone, excludeId: contact.Id))
            {
                _dialogService.ShowWarning("Контакт с таким номером уже существует!");
                return;
            }

            try
            {
                await _repository.UpdateAsync(contact);

                // Находим элемент в коллекции и уведомляем об изменении
                var existing = Contacts.FirstOrDefault(c => c.Id == contact.Id);
                if (existing != null)
                {
                    var index = Contacts.IndexOf(existing);
                    Contacts[index] = contact; // замена элемента с уведомлением UI
                }

                _dialogService.ShowInfo("Изменения сохранены");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка при сохранении: {ex.Message}");
            }
        }

        // Метод для обновления списка после возврата с экрана редактирования
        public async Task RefreshAfterEditAsync()
        {
            await LoadContactsAsync();
        }
    }
}