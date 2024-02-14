using System.ComponentModel.DataAnnotations;

namespace BooksAPI.DTOs.CategoryDtos;

public class AddCategoryDto
{
    public string Name { get; set; } = string.Empty;
}
