using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces;

public interface IBookInterface : IRepository<Book>
{
    Task<IEnumerable<Book>> GetBooksWithCategoryAsync();
}