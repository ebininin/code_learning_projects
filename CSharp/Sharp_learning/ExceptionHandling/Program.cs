using System;

namespace ExceptionHandling
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            int num1, num2;

            try
            {
                Console.Write("Enter a number: ");
                num1 = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter another number: ");
                num2 = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine(num1 / num2);
            }
            catch (DivideByZeroException e)
            {
                Console.WriteLine($"\nError: {e.Message}");
            }
            catch (FormatException e)
            {
                Console.WriteLine($"\nError: {e.Message}");
            }
        }
    }
}