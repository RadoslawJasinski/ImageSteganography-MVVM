using System;
using System.Globalization;
using System.Windows.Data;

namespace SteganographyApp.Conventers
{
    public class SizeInfoConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString().StartsWith("-"))
            {
                var info = "Brak miejsca";
                return info;
            }
            else if (value.ToString().Equals("0"))
            {
                var info = "-- ";
                return info;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
