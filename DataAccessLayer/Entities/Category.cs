namespace DataAccessLayer.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public List<Book> Books = new();
}