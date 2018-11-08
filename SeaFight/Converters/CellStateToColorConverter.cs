using System;
using System.Globalization;
using Xamarin.Forms;

using SeaFight.Enums;
using SeaFight.Models;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Converters
{
    public class CellStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cells = value as FieldCell[,];
            var indexes = parameter as (int, int)?;
            if (cells == null)
            {
                ErrorDetected($"Converted value is not {typeof(FieldCell[,]).Name} or value", ReasonType.NullError);
                return Colors.DefaultColor;
            }
            if (indexes == null)
            {
                ErrorDetected($"Converter parameter is not {nameof(Tuple<int, int>)} or parameter", ReasonType.NullError);
                return Colors.DefaultColor;
            }

            var state = cells[indexes.Value.Item1, indexes.Value.Item2]?.State;
            if (state == null)
            {
                ErrorDetected($"Evaluated state is not correct or ", ReasonType.NullError);
                return Colors.DefaultColor;
            }

            switch (state.Value)
            {
                case CellState.Idle: return Colors.Idle.Value;
                case CellState.IdleOvercovered: return Colors.IdleOvercovered.Value;
                case CellState.ShipAttacked: return Colors.ShipAttacked.Value;
                case CellState.ShipIdle: return Colors.ShipIdle.Value;
                case CellState.ShipOvercovered: return Colors.ShipOvercovered.Value;
                default: return Colors.DefaultColor.Value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(Extensions.StringExtension.NotSupportedMessage);
        }
    }
}
