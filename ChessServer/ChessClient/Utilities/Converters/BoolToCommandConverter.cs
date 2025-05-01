using ChessClient.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessClient.Utilities.Converters
{
    public class BoolToCommandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string parameters)
            {
                var parts = parameters.Split(';');
                return boolValue
                    ? new RelayCommand(() => ExecuteCommand(parts[0]))
                    : new RelayCommand(() => ExecuteCommand(parts[1]));
            }
            return null;
        }

        private void ExecuteCommand(string commandName)
        {
            if (Application.Current.MainPage?.BindingContext is AuthViewModel vm)
            {
                var command = vm.GetType()
                    .GetProperty(commandName)?
                    .GetValue(vm) as ICommand;

                command?.Execute(null);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
