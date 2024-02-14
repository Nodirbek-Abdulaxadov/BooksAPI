using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class Repository<TEntity>
    : IRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly AppDbContext _dbContext;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(TEntity entity)
        => await _dbContext.Set<TEntity>().AddAsync(entity);

    public void Delete(int id)
    {
        var entity = _dbContext.Set<TEntity>()
                               .FirstOrDefault(i =>
                                        i.Id == id);
        _dbContext.Set<TEntity>().Remove(entity);
    }

    public Task<IEnumerable<TEntity>> GetAllAsync()
    {
        var list = _dbContext.Set<TEntity>()
                             .AsNoTracking()
                             .AsEnumerable();
        return Task.FromResult(list);
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
        var entity = await _dbContext.Set<TEntity>()
                                     .FirstOrDefaultAsync(i =>
                                        i.Id == id);
        return entity;
    }

    public void Update(TEntity entity)
        => _dbContext.Set<TEntity>().Update(entity);
}