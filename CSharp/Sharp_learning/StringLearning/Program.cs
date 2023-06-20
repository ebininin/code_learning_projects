using System;

namespace StringLearning {
    internal class Program {
        public static void Main(string[] args) {
            string phrase = "Giraffe\"Academy", strr = "wot";
            Console.WriteLine(phrase.IndexOf("a"));
            Console.WriteLine(phrase.Length);
            Console.WriteLine(phrase.Substring(3, 7));
            Console.WriteLine(phrase.Contains("a"));
            Console.WriteLine(phrase[0]);
        }
    }
}