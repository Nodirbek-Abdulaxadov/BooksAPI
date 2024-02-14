using System.ComponentModel.DataAnnotations;

namespace BooksAPI.DTOs.BookDtos;

public class AddBookDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}