using System;

namespace Cyclops
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            int a = 1448045611;
            Console.WriteLine(IsCyclops(a));
        }

        static bool IsCyclops(int num)
        {
            int digit, power, cnt;
            digit = 0;
            power = 0;
            cnt = 0;

            while (num > power)
            {
                digit += 1;
                power *= 10;
                cnt += 1;
                num = (num % power) / 10;
                power /= 100;
            }

            return cnt % 2 == 1 && num == 0;
        }
    }
}