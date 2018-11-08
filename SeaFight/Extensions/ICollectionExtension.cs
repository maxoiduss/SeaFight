using System;
using System.Linq;
using System.Collections.Generic;

using SeaFight.Enums;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Extensions
{
    public static class ICollectionExtension
    {
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            try
            {
                if (target == null)
                    throw new ArgumentNullException(nameof(target));
                if (source == null)
                    throw new ArgumentNullException(nameof(source));
                
                foreach (var element in source)
                    target.Add(element);
            }
            catch (Exception ex)
            {
                ErrorDetected(ex.Message, ReasonType.ICollectionError);
                ErrorDetected(ex.Message, ReasonType.Exception);
            }
        }
    }
}
