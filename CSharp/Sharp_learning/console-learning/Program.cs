using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_learning {
    class Program {
        static void Main(string[] args) {
            string characterName = "John";
            int characterAge = 730;

            Console.WriteLine($"There once a man named {characterName}");
            Console.WriteLine($"He was {characterAge} years old");

            characterName = "Boots";
            
            Console.WriteLine($"He really like the name {characterName}");
            Console.WriteLine($"But didn't like being {characterAge}");

            Console.ReadLine();
        }
    }
}