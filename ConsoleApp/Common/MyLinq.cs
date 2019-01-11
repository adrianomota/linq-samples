using System.Collections.Generic;

namespace ConsoleApp.Common
{
    internal static class MyLinq
    {
        public static int Count<T>(this IEnumerable<T> sequence)
        {
            int count = 0;

            foreach (T item in sequence)
            {
                count++;
            }

            return count;
        }

           
        }

}