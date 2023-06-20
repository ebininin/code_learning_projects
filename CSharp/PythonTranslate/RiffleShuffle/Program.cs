using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RiffleShuffle
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            int[] a = { 1, 2, 3, 4, 5, 6, 7, 8 };
            Console.WriteLine(string.Join(", ", Riffle(a, true)));
        }

        static List<int> Riffle(int[] items, bool outValue=true)
        {
            int itemsLen = items.Length;
            List<int> finalList = new List<int>(itemsLen);
            int itemsLenHalves = itemsLen / 2;

            for (int i = 0; i < itemsLenHalves; i++)
            {
                finalList.Add(items[i]);
                finalList.Add(items[i + itemsLenHalves]);
            }

            if (!outValue)
            {
                finalList.Reverse();
            }
            return finalList;
        }
    }
}