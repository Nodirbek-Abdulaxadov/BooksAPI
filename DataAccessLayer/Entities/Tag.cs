namespace DataAccessLayer.Entities;
public class Tag : BaseEntity
{
    public string Name { get; set; }    = null!;
    public List<Book> Books { get; set; } = new();
}