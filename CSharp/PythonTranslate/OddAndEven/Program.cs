using System;

namespace OddAndEven
{
    internal class Program
    {
        public static void Main(string[] args)
        {
             int a = 713599513;
            Console.WriteLine(OnlyOddDigits(a));
        }

        static bool OnlyOddDigits(int num)
        {
            string numStr = num.ToString();
            // for (int i = 0; i < numStr.Length; i++)
            foreach (char n in numStr)
            {
                int digit = int.Parse(n.ToString());
                if (digit % 2 == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}