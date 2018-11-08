using System;
using System.Globalization;
using Xamarin.Forms;

using SeaFight.Enums;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Converters
{
    public class LengthMultiplierConverter : /*BindableObject,*/ IValueConverter
    {
        public double Addition { get; set; } = 0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var length = value as double?;
            if (length is null)
            {
                ErrorDetected($"Converted value is not {typeof(double).Name} or value", ReasonType.NullError);
                return 1.0;
            }
            var multiplier = parameter as double?;
            if (multiplier is null)
            {
                ErrorDetected($"Converter parameter is not {typeof(double).Name} or parameter", ReasonType.NullError);
                return length;
            }

            return length * multiplier + Addition;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(Extensions.StringExtension.NotSupportedMessage);
        }
    }
}
