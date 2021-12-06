using System.Linq;

namespace Signals.MvcExample.Data
{
    public interface IDataRepository
    {
        Book[] Books { get; }     
        Book GetBook(int id);
    }

    public class DataRepository : IDataRepository
    {       

        private readonly Book[] _books = new[]
        {
            new Book(1,"Grace under Pressure","Alex Lifeson",6.99m, 4),
            new Book(2,"Clockwork Angels","Geddy Lee", 16.99m, 5),
            new Book(3,"Power Windows","Neil Peart", 9.99m, 2)
        };

        public Book[] Books => _books;
        public Book GetBook(int id) => _books.FirstOrDefault(x => x.Id == id);
       


    }

  
    public class Book
    {
        public Book(int id, string title, string author, decimal price, int weight)
        {
            Id = id;
            Title = title;
            Author = author;
            Price = price;
            Weight = weight;
        }

        public int Id { get; }
        public string Title { get; }
        public string Author { get; }
        public decimal Price { get; }
        public int Weight { get; }

        public override string ToString()
        {
            return $"{Title} {Price}";
        }
    }
}
