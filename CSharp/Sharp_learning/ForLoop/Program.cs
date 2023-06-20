using System;

namespace ForLoop {
    internal class Program {
        public static void Main(string[] args) {
            int[] luckyNumbers = { 4, 5, 6, 5, 2, 234, 4234, 323, 478, 45 };

            foreach (int item in luckyNumbers) {
                Console.WriteLine(item);
            }
        }
    }
}