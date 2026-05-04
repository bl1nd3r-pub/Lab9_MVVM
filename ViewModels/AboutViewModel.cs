using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab11_Navigation.ViewModels
{
    public class AboutViewModel : ObservableObject
    {
        public string AppName => "Телефонная книга MVVM";
        public string Version => "Версия 2.0 (Navigation)";
    }
}
