using System;
using System.Linq;
using System.Globalization;
using Xamarin.Forms;

using SeaFight.Enums;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Converters
{
    public class StringsToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;
            if (val is null)
            {
                ErrorDetected($"Converted value is not {typeof(string).Name} or value", ReasonType.NullError);
                return false;
            }
            var param = parameter as string[];
            if (param is null)
            {
                ErrorDetected($"Converter parameter is not {typeof(string[]).Name} or parameter", ReasonType.NullError);
                return false;
            }

            return !string.IsNullOrEmpty(param.FirstOrDefault((str) => str.Equals(val, StringComparison.OrdinalIgnoreCase)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(Extensions.StringExtension.NotSupportedMessage);
        }
    }
}
