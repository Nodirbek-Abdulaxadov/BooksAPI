using BooksAPI.DTOs.BookDtos;
using DataAccessLayer.Entities;

namespace BooksAPI.DTOs.CategoryDtos;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<BookDto> Books = new();
}
