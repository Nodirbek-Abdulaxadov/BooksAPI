namespace BooksAPI.Tests.DataAccessLayer;

internal static class DbContextHelper
{
    private static readonly DbContextOptions<AppDbContext> options =
        new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: "BooksDB")
        .Options;

    public static AppDbContext GetDbContext()
        => new AppDbContext(options);

    public static IUnitOfWork GetUnitOfWork()
        => new UnitOfWork(GetDbContext());
}