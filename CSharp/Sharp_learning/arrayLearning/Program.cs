using System;

namespace arrayLearning {
    internal class Program {
        public static void Main(string[] args) {
            int[] luckyNumber = { 213231, 543423, 452343, 564563423, 567845223, 454523434, 454531234 };
            luckyNumber[3] = 333323;
            Console.WriteLine(luckyNumber[4]);
            Console.WriteLine(luckyNumber[3]);

            string[] friends = new string[5];
            friends[0] = "Kyle";
            friends[1] = "Butter";
            Console.WriteLine(luckyNumber);
            Console.WriteLine(friends);
        }
    }
}