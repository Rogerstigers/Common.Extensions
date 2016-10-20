using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Common.Extensions
{
    public static class CollectionExtensions
    {
        #region Public Methods

        #region HasItems
        /// <summary>
        /// Returns true if the ICollection is not null and it has items
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool HasItems<T>(this ICollection<T> collection)
        {
            return collection != null && collection.Count != 0;
        }
        #endregion

        #region Join
        public static string Join(this NameValueCollection c, Func<string, string> selector, string separator)
        {
            return c != null
                        ? string.Join(separator, c.Cast<string>().Select(selector))
                        : string.Empty;
        }
        #endregion

        #region Shuffle
        public static List<T> Shuffle<T>(this IList<T> source)
        {
            if (source == null)
                return null;

            var list = new List<T>(source);

            if (list.Count < 2)
                return list;

            if (list.Count == 2)
                Swap(list, 0, 1);
            else
            {
                var random = new Random();
                for (var index = list.Count - 1; index > 1; index--)
                {
                    var randomIndex = random.Next(0, index);
                    Swap(list, index, randomIndex);
                }
            }

            return list;
        }
        #endregion

        #endregion

        #region Private Methods

        #region Swap
        private static void Swap<T>(IList<T> items, int index1, int index2)
        {
            var temp = items[index1];
            items[index1] = items[index2];
            items[index2] = temp;
        }
        #endregion

        #endregion
    }
}
