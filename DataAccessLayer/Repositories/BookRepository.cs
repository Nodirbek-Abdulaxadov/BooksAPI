using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class BookRepository
    : Repository<Book>, IBookInterface
{
    public BookRepository(AppDbContext dbContext)
        : base(dbContext) { }

    public async Task<IEnumerable<Book>> GetBooksWithCategoryAsync()
        => await _dbContext.Books
                           .Include(b => b.Category)
                           .Include(b => b.Tags)
                           .ToListAsync();
}
