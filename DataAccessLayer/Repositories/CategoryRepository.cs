using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class CategoryRepository
    : Repository<Category>, ICategoryInterface
{
    public CategoryRepository(AppDbContext dbContext) 
        : base(dbContext)
    {
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesWithBooksAsync()
        => await _dbContext.Categories
                           .Include(c => c.Books)
                           .ToListAsync();
}