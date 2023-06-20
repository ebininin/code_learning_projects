using System;
using System.Globalization;

namespace UserInputLearning {
    internal class Program {
        public static void Main(string[] args) {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine($"Hello {name}");
        }
    }
}