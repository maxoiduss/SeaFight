using System.Collections.Generic;

using SeaFight.Enums;
using SeaFight.Models;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Helpers
{
    public static class PropertyHandlerHelper<TValue>
        where TValue : struct
    {
        public static List<TValue> DefaultValueList { get; set; } = new List<TValue>();

        public static void HandlePropertyChanging(object oldValue, object newValue, string propertyName, bool isWeak = false)
        {
            if (isWeak) return;
            if (oldValue == newValue) return;

            if (propertyName == nameof(XamlHelper<TValue>.Name))
            {
                var name = (newValue as string);

                if (name == null)
                {
                    ErrorDetected($"Error in {nameof(HandlePropertyChanging)} of {nameof(XamlHelper<TValue>.Name)}: {nameof(newValue)}", ReasonType.NullError);
                    return;
                }
                if (string.IsNullOrWhiteSpace(name))
                    return;
                // FieldColor handling implementation
                FieldColor.HandleFieldColorBuffer(name, true);
            }

            else if (propertyName == nameof(XamlHelper<TValue>.Value))
            {
                var value = (newValue as TValue?);

                if (value == null)
                {
                    ErrorDetected($"Error in {nameof(HandlePropertyChanging)} of {nameof(XamlHelper<TValue>.Value)}: {nameof(newValue)}", ReasonType.NullError);
                    return;
                }
                if (!DefaultValueList.TrueForAll((item) => !item.Equals(value)))
                    return;
                // FieldColor handling implementation
                FieldColor.HandleFieldColorBuffer(value, false);
            }
        }
    }
}
