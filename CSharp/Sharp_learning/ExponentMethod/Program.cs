using System;

namespace ExponentMethod {
    internal class Program {
        public static void Main(string[] args) {
            int[,] numberGrid = {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };
            
            Console.WriteLine(numberGrid[2,1]);
        }
    }
}