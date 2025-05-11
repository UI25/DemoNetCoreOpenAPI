using BooksWebAPI.Models;

namespace BooksWebAPI.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(Guid id);
        Task<Book?> AddBookAsync(Book book);
        Task<Book?> UpdateBookAsync(Guid id, Book book);
        Task<bool> DeleteBookAsync(Guid id);
    }
}
