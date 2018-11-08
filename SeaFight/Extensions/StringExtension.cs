using System.Collections.Generic;

using SeaFight.Models;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Extensions
{
    public static class StringExtension
    {
        public static readonly string NotSupportedMessage = "converting back is not supported";

        public static readonly string[] SupportedStatusStrings = { Statuses.GameFinished, Statuses.GameFinishedLuck, Statuses.GameFinishedUnLuck, Statuses.RestartGame };

        public static string AggregateStringWith(this string str, object toAggregate)
        {
            if (str == null)
            {
                ErrorDetected($"{nameof(AggregateStringWith)} error: target string", Enums.ReasonType.NullError);
                return string.Empty;
            }
            if (toAggregate == null)
            {
                ErrorDetected($"{nameof(AggregateStringWith)} error: {nameof(toAggregate)}", Enums.ReasonType.NullError);
                return str;
            }

            return str + toAggregate;
        }

        public static IEnumerable<string> GenerateAggregatedStrings(this string str, int startIndex, int finishIndex)
        {
            if (finishIndex < startIndex)
            {
                ErrorDetected($"{nameof(GenerateAggregatedStrings)} error: {nameof(startIndex)} or {nameof(startIndex)} is incorrect.");
                yield break;
            }

            for (int i = startIndex; i <= finishIndex; ++i)
            {
                yield return str.AggregateStringWith(i);
            }
        }
    }
}
