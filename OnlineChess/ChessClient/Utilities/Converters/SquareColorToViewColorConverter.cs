using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessClient.Models.Board;
namespace ChessClient.Utilities.Converters
{
    public class SquareColorToViewColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SquareColor boolValue)
            {
                return boolValue == SquareColor.Black ? Color.FromArgb("#000000") : Color.FromArgb("#FFFFFF");
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
