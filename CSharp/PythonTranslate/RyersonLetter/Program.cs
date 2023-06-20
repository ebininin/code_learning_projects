using System;
using System.Collections.Generic;

namespace RyersonLetter {
    internal class Program {
        public static void Main(string[] args) {
            Console.Write("Score input (0-100): ");
            int score = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine(ScoreScale(score));
        }

        static string ScoreScale(int score) {
            List<Tuple<int, int, string>> scoreDict = new List<Tuple<int, int, string>> {
                Tuple.Create(90, 101, "A+"),
                Tuple.Create(85, 90, "A"),
                Tuple.Create(80, 85, "A-"),
                Tuple.Create(77, 80, "B+"),
                Tuple.Create(73, 77, "B"),
                Tuple.Create(70, 73, "B-"),
                Tuple.Create(67, 70, "C+"),
                Tuple.Create(63, 67, "C"),
                Tuple.Create(60, 63, "C-"),
                Tuple.Create(57, 60, "D+"),
                Tuple.Create(53, 57, "D"),
                Tuple.Create(50, 53, "D-"),
                Tuple.Create(0, 50, "F")
            };
            foreach (var item in scoreDict) {
                if (score >= item.Item1 && score <= item.Item2) {
                    return item.Item3;
                }
            }
            return "Disgraded";
        }
    }
}