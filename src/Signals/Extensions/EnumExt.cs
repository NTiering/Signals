using System.Collections.Generic;
using System.Linq;

namespace Signals.Extensions
{
    public static class EnumExt
    {
        /// <summary>
        /// If an enumeration is null, returns an empty one instead, otherwise just return he enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Returns true if the enumeration is empty OR null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool Empty<T>(this IEnumerable<T> collection)
        {
            return collection == null || collection.Any() == false;
        }
    }
}
