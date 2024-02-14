using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities;

public class Book : BaseEntity
{
    [Required, StringLength(50)]
    public string Title { get; set; } = string.Empty;
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;
    [Required, StringLength(50)]
    public string Author { get; set; } = string.Empty;
    [Required]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }
    public Category Category = new();

    public List<Tag> Tags { get; set; } = new();
}