using BooksAPI.DTOs.BookDtos;
using BusinessLogicLayer.Helpers;
namespace BusinessLogicLayer.Interfaces;

public interface IBookService
{
    Task<PagedList<BookDto>> Filter(FilterParametrs parametrs);
    Task<List<BookDto>> GetBooksAsync();
    Task<BookDto> GetBookByIdAsync(int id);
    Task AddBookAsync(AddBookDto newBook);
    Task UpdateBookAsync(BookDto BookDto);
    Task DeleteBookAsync(int id);
    Task AddTestAsync();
}
