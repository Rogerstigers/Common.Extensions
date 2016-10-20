using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// The items in ls are passed one by one to 'pred'.  When 'pred' returns true, true if returned from this function.
        /// If 'pred' never returns true, false is returned from this function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ls">The ls.</param>
        /// <param name="pred">The pred.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified ls]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains<T>(this IEnumerable<T> ls, Func<T, bool> pred) => ls.Any(pred);

        /// <summary>
        /// Returns a new list that has been sorted using the given comparison function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ls">The ls.</param>
        /// <param name="cmp">The CMP.</param>
        /// <returns></returns>
        public static List<T> Sort<T>(this IEnumerable<T> ls, Comparison<T> cmp)
        {
            var result = new List<T>(ls);
            result.Sort(cmp);
            return result;
        }

        /// <summary>
        /// Sorts the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public static IEnumerable<T> Sort<T>(Func<IEnumerable<T>> action, IComparer<T> comparer) => action().Sort(comparer.Compare);

        /// <summary>
        /// Sorts the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="id">The id.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public static IEnumerable<T> Sort<T>(Func<int, IEnumerable<T>> action, int id, IComparer<T> comparer) => action(id).Sort(comparer.Compare);

        /// <summary>
        /// Runs 'p' on each item in ls, in order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ls">The ls.</param>
        /// <param name="p">The p.</param>
        public static void ForEach<T>(this IEnumerable<T> ls, Action<T> p)
        {
            foreach (var t in ls) p(t);
        }

        /// <summary>
        /// Runs 'p' on each item in ls, in order.  Passes the zero based index of the item along with the item
        /// itself to 'p'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ls">The ls.</param>
        /// <param name="p">The p.</param>
        public static void ForEach<T>(this IEnumerable<T> ls, Action<int, T> p)
        {
            var i = 0;
            foreach (var t in ls) p(i++, t);
        }

        /// <summary>
        /// Runs 'p' on each pair of items from 'als' and 'bls', pairwise, in order.  Throws an ArgumentException
        /// if the lists are different sizes.
        /// </summary>
        /// <typeparam name="TA"></typeparam>
        /// <typeparam name="TB"></typeparam>
        /// <param name="als">The als.</param>
        /// <param name="bls">The BLS.</param>
        /// <param name="p">The p.</param>
        public static void ForEach<TA, TB>(IEnumerable<TA> als, IEnumerable<TB> bls, Action<TA, TB> p)
        {
            using (var a = als.GetEnumerator())
            {
                using (var b = bls.GetEnumerator())
                {
                    while (true)
                    {
                        var success = a.MoveNext();
                        if (success != b.MoveNext()) throw new ArgumentException("enumerations differ in length");
                        if (!success) break;
                        p(a.Current, b.Current);
                    }
                }
            }
        }

        public static List<List<T>> Split<T>(this List<T> items, int groupSize)
        {
            var list = new List<List<T>>();
            for (var i = 0; i < items.Count; i += groupSize)
                list.Add(items.GetRange(i, Math.Min(groupSize, items.Count - i)));
            return list;
        }
    }
}
