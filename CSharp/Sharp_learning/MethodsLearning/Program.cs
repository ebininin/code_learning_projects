using System;

namespace MethodsLearning {
    internal class Program {
        public static void Main(string[] args) {
            string name1 = Console.ReadLine();
            Greeting(name1);
        }

        static void Greeting(string user) {
            Console.WriteLine($"Hello {user}");
        }
        
    }
}