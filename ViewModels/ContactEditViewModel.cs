using Lab11_Navigation.Interfaces;
using Lab11_Navigation.Models;
using Lab11_Navigation.Repositories;  // Добавь этот using!
using Lab11_Navigation.Services;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lab11_Navigation.ViewModels
{
    public class ContactEditViewModel : ObservableObject, INavigationAware
    {
        private readonly IContactRepository _repository;  // Добавляем репозиторий!
        private readonly INavigationService _navigation;
        private readonly IDialogService _dialogService;   // Для сообщений
        private Contact _contact = null!;

        public string EditName
        {
            get => _contact.Name;
            set
            {
                _contact.Name = value;
                OnPropertyChanged();
                ((RelayCommand)SaveCommand).NotifyCanExecuteChanged(); // Обновляем CanExecute
            }
        }

        public string EditPhone
        {
            get => _contact.Phone;
            set
            {
                _contact.Phone = value;
                OnPropertyChanged();
                ((RelayCommand)SaveCommand).NotifyCanExecuteChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Добавляем IDialogService в конструктор!
        public ContactEditViewModel(
            IContactRepository repository,
            INavigationService navigation,
            IDialogService dialogService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(async () => await SaveAsync(), CanSave);
            CancelCommand = new RelayCommand(() =>
                _navigation.NavigateTo<ContactsListViewModel>());
        }

        private bool CanSave() => _contact?.Validate() == true;

        private async Task SaveAsync()
        {
            if (_contact == null || !_contact.Validate())
                return;

            // Проверяем на дубликат телефона (исключая текущий контакт)
            if (await _repository.ExistsByPhoneAsync(_contact.Phone, excludeId: _contact.Id))
            {
                _dialogService.ShowWarning("Контакт с таким номером уже существует!");
                return;
            }

            try
            {
                await _repository.UpdateAsync(_contact);
                _dialogService.ShowInfo("Изменения сохранены");
                _navigation.NavigateTo<ContactsListViewModel>();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка при сохранении: {ex.Message}");
            }
        }

        public void OnNavigatedTo(object? parameter)
        {
            if (parameter is Contact contact)
            {
                _contact = contact;
                // Уведомляем UI об изменении свойств
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