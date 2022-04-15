using BooksAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }
    public DbSet<Book> Books { get; set; }
}
