using System;
using System.Collections.Generic;
using System.Linq;

namespace DominoCycle
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            List<(int, int)> a = new List<(int, int)>
            {
                (5, 2),
                (2, 3),
                (4, 5)
            };
            Console.WriteLine(DominoCycle(a));
        }

        static bool DominoCycle(List<(int, int)> tiles)
        {
            if (tiles == null || tiles.Count == 0)
            {
                return false;
            }

            int firstNum = tiles[0].Item1;
            int lastNum = tiles[0].Item2;

            for (int i = 1; i < tiles.Count; i++)
            {
                var domino = tiles[i];

                if (domino.Item1 != lastNum)
                {
                    return false;
                }
                lastNum = domino.Item2;
            }
            return lastNum == firstNum;
        }
    }
}