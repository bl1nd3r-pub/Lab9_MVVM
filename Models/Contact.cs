using System;
using System.Collections.Generic;

namespace Lab11_Navigation.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public Contact() { }

    public Contact(string name, string phone) {
        Name = name;
        Phone = phone;
    }


    public bool Validate()
    {

        bool isNameValid = !string.IsNullOrWhiteSpace(Name);

        bool isPhoneValid = !string.IsNullOrWhiteSpace(Phone) &&
            ((Phone.StartsWith("+7") && Phone.Length == 12) ||
            (Phone.Length == 10));

        return (isNameValid && isPhoneValid);
    }
}

