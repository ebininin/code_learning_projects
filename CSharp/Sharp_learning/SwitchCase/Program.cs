using System;

namespace SwitchCase {
    internal class Program {
        public static void Main(string[] args) {
            Console.Write("Enter the number: ");
            int day = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine(GetDay(day));
        }

        static string GetDay(int dayNum) {
            string dayName;
            
            switch (dayNum) {
                case 0:
                    dayName = "Sunday";
                    break;
                case 1:
                    dayName = "Monday";
                    break;
                case 2:
                    dayName = "Tuesday";
                    break;
                case 3:
                    dayName = "Wednesday";
                    break;
                case 4:
                    dayName = "Thursday";
                    break;
                case 5:
                    dayName = "Friday";
                    break;
                case 6:
                    dayName = "Saturday";
                    break;
                default:
                    dayName = "Invalid Number";
                    break;
            }
            return dayName;
        }
    }
}