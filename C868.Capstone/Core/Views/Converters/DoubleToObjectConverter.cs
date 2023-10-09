using System;
using System.Globalization;
using System.Windows.Data;

namespace C868.Capstone.Core.Views.Converters
{
    public class DoubleToObjectConverter : IValueConverter
    {
        // value > 0  => PositiveObject
        // value == 0 => ZeroObject
        // value < 0  => NegativeObject
        public object PositiveObject { get; set; }
        public object ZeroObject { get; set; }
        public object NegativeObject { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return doubleValue > 0d
                    ? PositiveObject
                    : doubleValue < 0d
                        ? NegativeObject
                        : ZeroObject;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0d;
        }
    }
}