using System;
using System.Globalization;
using Xamarin.Forms;

namespace SeaFight.Converters
{
    public class LengthDividerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int denominator;
            double length;
            denominator = (value as int?) ?? 1;

            if (parameter is VisualElement element)
                length = (int)element.Width;
            else
            {
                if (parameter is int _length)
                    length = _length > 0 ? _length : 1.0;
                else if (parameter is double __length)
                    length = __length > 0.0 ? __length : 1.0;
                else
                    length = 1.0;
            }

            return denominator > 0 ? length / denominator : length;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(Extensions.StringExtension.NotSupportedMessage);
        }
    }
}
