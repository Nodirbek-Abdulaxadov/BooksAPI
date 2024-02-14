using System.ComponentModel.DataAnnotations;

namespace BooksAPI.DTOs.UserDtos;

public class LoginUserDto
{
    [Required, MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}
