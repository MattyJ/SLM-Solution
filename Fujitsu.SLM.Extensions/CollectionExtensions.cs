using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Extensions
{
    public static class CollectionExtensions
    {
        public static List<T> RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            var removed = new List<T>();

            for (var i = 0; i < collection.Count; i++)
            {
                T element = collection.ElementAt(i);
                if (predicate(element))
                {
                    collection.Remove(element);
                    removed.Add(element);
                    i--;
                }
            }

            return removed;
        }
    }
}