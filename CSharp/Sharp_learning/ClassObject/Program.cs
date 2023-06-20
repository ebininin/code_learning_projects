using System;

namespace ClassObject
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Book book1 = new Book("Harry Potter", "JKR", 400);
            Book book2 = new Book("Lord of the Rings", "Tolkein", 700);

            Console.WriteLine(book1.Author);
            Console.WriteLine(book2.Author);

        }
    }
}