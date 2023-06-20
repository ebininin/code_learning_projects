using System;

namespace ReturnStatementLearning {
    internal class Program {
        public static void Main(string[] args) {
            int a = 34123;
            Console.WriteLine(Cube(a));
        }

        static double Cube(double num) {
            double result = Math.Pow(num, 3);
            return result;
        }
    }
}