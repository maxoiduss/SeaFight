using SeaFight.Enums;
using SeaFight.Models;
using static SeaFight.Models.Colors;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Extensions
{
    public static class FieldColorExtension
    {
        public static bool IsDefaultOrNull(this FieldColor fieldColor, bool checkForDefaultFieldColor = true)
        {
            if (fieldColor is null)
            {
                ErrorDetected($"Error in {nameof(IsDefaultOrNull)}: target {fieldColor}", ReasonType.NullError);
                return true;
            }

            return (string.IsNullOrEmpty(fieldColor.Name)
                    && fieldColor.Value == default(Xamarin.Forms.Color)
                    && (!checkForDefaultFieldColor || fieldColor.Value == DefaultColor));
        }

        public static bool HasDefaultColorOrName(this FieldColor fieldColor, bool checkForDefaultFieldColor = true)
        {
            if (fieldColor is null)
            {
                ErrorDetected($"Error in {nameof(IsDefaultOrNull)}: target {fieldColor}", ReasonType.NullError);
                return true;
            }

            return (string.IsNullOrEmpty(fieldColor.Name)
                    || fieldColor.Value == default(Xamarin.Forms.Color)
                    || (checkForDefaultFieldColor && fieldColor.Value == DefaultColor));
        }

        public static bool HasDefaultColorOrNull(this FieldColor fieldColor, bool checkForDefaultFieldColor = true)
        {
            if (fieldColor is null)
            {
                ErrorDetected($"Error in {nameof(IsDefaultOrNull)}: target {fieldColor}", ReasonType.NullError);
                return true;
            }

            return (fieldColor.Value == default(Xamarin.Forms.Color)
                    || (checkForDefaultFieldColor && fieldColor.Value == DefaultColor));
        }

        public static bool HasDefaultNameOrNull(this FieldColor fieldColor)
        {
            if (fieldColor is null)
            {
                ErrorDetected($"Error in {nameof(IsDefaultOrNull)}: target {fieldColor}", ReasonType.NullError);
                return true;
            }

            return (string.IsNullOrEmpty(fieldColor.Name));
        }
    }
}
