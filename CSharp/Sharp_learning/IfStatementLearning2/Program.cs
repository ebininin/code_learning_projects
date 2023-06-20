using System;

namespace IfStatementLearning2 {
    internal class Program {
        public static void Main(string[] args) {
            int a = 1;
            int b = 23;
            Console.WriteLine(GetMax(a, b));
        }

        static int GetMax(int num1, int num2) {
            int result;
            if (num1 > num2) {
                result = num1;
            }
            else {
                result = num2;
            }
            return result;
        }
    }
}