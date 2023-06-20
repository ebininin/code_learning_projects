using System;

namespace ClassObject
{
    public class Book
    {
        public string Title, Author;
        public int Pages;

        public Book()
        {
        }

        public Book(string aTitle, string aAuthor, int aPages)
        {
            Title = aTitle;
            Author = aAuthor;
            Pages = aPages;
        }
    }
}