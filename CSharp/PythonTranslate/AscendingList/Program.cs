using System;
using System.Linq;

namespace AscendingList
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            int[] a = { };
            Console.WriteLine(IsAscending(a));
        }

        static bool IsAscending(int[] items)
        {
            for (int i = 1; i < items.Length; i++)
            {
                if (items[i] <= items[i - 1])
                {
                    return false;
                }
            }

            return true;
        }
    }
}