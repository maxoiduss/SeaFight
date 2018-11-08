using Xamarin.Forms;

using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Extensions
{
    public static class ResourceDictionaryExtension
    {
        public static bool ContainsKeys(this ResourceDictionary dictionary, params string[] keys)
        {
            if (dictionary == null)
            {
                ErrorDetected($"{nameof(ContainsKeys)} error: target dictionary", Enums.ReasonType.NullError);
                return false;
            }

            foreach (var key in keys)
            {
                if (!dictionary.ContainsKey(key))
                    return false;
            }

            return true;
        }
    }
}
