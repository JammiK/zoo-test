using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Core.JG.Extensions
{
    public static class ArrayExtension
    {
        public static void Shuffle<T> (this ICollection[] array, int seed)
        {
            var random = new Random(seed);
            var n = array.Length;
            while (n > 1)
            {
                var k = random.Next(0, n--);
                var temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
        
        public static IEnumerable<T> ShuffleCopy<T> (this IEnumerable<T> collection, int seed)
        {
            var random = new Random(seed);
            var array = collection.ToArray();
            var n = array.Length;
            while (n > 1)
            {
                var k = random.Next(0, n--);
                var temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }

            return array;
        }

        public static string ArrayToString(this IEnumerable list, string separator)
        {
            return string.Join(separator, list.ArrayToStrings());
        }

        public static string[] ArrayToStrings(this IEnumerable list)
        {
            if (list == null)
                return new[] { "null" };

            List<string> cache = new List<string>();

            foreach (object obj in list)
                cache.Add(obj == null ? "null" : obj.ToString());

            string[] strings = cache.ToArray();
            cache.Clear();

            return strings;
        }
    }
}