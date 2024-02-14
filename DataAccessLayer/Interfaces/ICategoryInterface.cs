using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces;

public interface ICategoryInterface 
    : IRepository<Category>
{
    Task<IEnumerable<Category>> GetAllCategoriesWithBooksAsync();
}