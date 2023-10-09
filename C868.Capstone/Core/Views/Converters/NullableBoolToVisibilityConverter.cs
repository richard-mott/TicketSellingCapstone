using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace C868.Capstone.Core.Views.Converters
{
    public class NullableBoolToVisibilityConverter : IValueConverter
    {
        // true  => Visible
        // null  => Collapsed
        // false => Collapsed
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Visibility.Collapsed;
            }

            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return false;
            }

            return (Visibility)value == Visibility.Visible;
        }
    }
}