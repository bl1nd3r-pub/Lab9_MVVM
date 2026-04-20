using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab9_MVVM.ViewModels;

namespace Lab9_MVVM.Models
{
    public class Contact : ObservableObject
    {
        private string _name = string.Empty;
        private string _phone = string.Empty;
        public Contact(string name, string phone)
        {
            _name = name;
            _phone = phone;
            if (!Validate()) { throw new ArgumentException("Некорректные данные контакта"); }
        }
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
        public bool Validate() {

            bool isNameValid = !string.IsNullOrWhiteSpace(_name);

            bool isPhoneValid = !string.IsNullOrWhiteSpace(_phone) &&
                ((_phone.StartsWith("+7") && _phone.Length == 12) ||
                (_phone.Length == 10));

            return (isNameValid && isPhoneValid);
        }
    }
}
