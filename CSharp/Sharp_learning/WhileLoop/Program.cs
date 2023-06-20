using System;

namespace WhileLoop {
    internal class Program {
        public static void Main(string[] args) {
            int inDex = 101;
            do {
                Console.WriteLine(inDex);
                inDex++;
                Console.WriteLine(inDex);
            }
            while (inDex < 100);
            // inDex++;
        }
    }
}