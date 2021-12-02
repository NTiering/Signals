using System.Collections.Generic;
using System.Linq;

namespace Signals.Extensions
{
    public static class EnumExt
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        public static bool Empty<T>(this IEnumerable<T> collection)
        {
            return collection == null || collection.Any() == false;
        }
    }
}
