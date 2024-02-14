namespace DataAccessLayer.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICategoryInterface CategoryInterface { get; }
    IBookInterface BookInterface { get; }
    Task SaveAsync();
}