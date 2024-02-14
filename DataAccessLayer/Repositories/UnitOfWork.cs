using DataAccessLayer.Interfaces;

namespace DataAccessLayer.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ICategoryInterface CategoryInterface => new CategoryRepository(_dbContext);

    public IBookInterface BookInterface => new BookRepository(_dbContext);

    public void Dispose()
        => GC.SuppressFinalize(this);

    public async Task SaveAsync()
        => await _dbContext.SaveChangesAsync();
}
