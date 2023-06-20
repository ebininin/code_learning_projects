using System;

namespace ObjectMethods
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Student student1 = new Student("Kyle", "Preacher", 2.9);
            Student student2 = new Student("Cartman", "Con", 3.1);
            Student student3 = new Student("Kenny", "Poor", 3.7);
            
            Console.WriteLine(student1.IsHonored());
            Console.WriteLine(student2.IsHonored());
            Console.WriteLine(student3.IsHonored());
        }
    }
}