using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChessClient.ViewModels
{
    public partial class GameTimeOption : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _icon;
        public GameTimeOption(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }

        public bool IsSelected =>
            (Application.Current?.MainPage?.BindingContext is MainViewModel vm) &&
            (vm.SelectedTime == this);
    }
}
