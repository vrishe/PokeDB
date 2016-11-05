using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace PokeDB.Forms
{
    class StringFormatOneWayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format(culture, parameter as string ?? string.Empty, 
                (value as Array)?.Cast<object>().ToArray() ?? value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
