using System;

namespace IfStatementLearning {
    internal class Program {
        public static void Main(string[] args) {
            bool isMale = true;
            bool isTall = false;

            if (isMale && isTall) {
                Console.WriteLine("You half pig");
            }
            else if (isMale && !isTall) {
                Console.WriteLine("You fucking pig");
            }
            else {
                Console.WriteLine("Hmmmmmmmmmmmmmmmmmmmmm");
            }
        }
    }
}