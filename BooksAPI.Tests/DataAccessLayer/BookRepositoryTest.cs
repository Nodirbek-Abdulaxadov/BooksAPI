namespace BooksAPI.Tests.DataAccessLayer;

internal class BookRepositoryTest
{
    AppDbContext dbContext;
    IUnitOfWork unitOfWork;

    [SetUp]
    public void Setup()
    {
        dbContext = DbContextHelper.GetDbContext();
        unitOfWork = DbContextHelper.GetUnitOfWork();
    }

    [Test]
    public async Task AddAsync()
    {
        // Arrange
        var book = new Book
        {
            Id = 1,
            Title = "Book 1",
            Author = "Author 1",
            Price = 100,
            Description = "Description 1",
            CategoryId = 1,
            Category = null
        };
        await unitOfWork.BookInterface.AddAsync(book);
        await unitOfWork.SaveAsync();

        // Act
        var result = await unitOfWork.BookInterface.GetByIdAsync(book.Id);

        // Assert
        Assert.That(result, Is.EqualTo(book));
    }

    [Test]
    public async Task UpdateAsync()
    {
        // Arrange
        var book = new Book
        {
            Id = 2,
            Title = "Book 2",
            Author = "Author 2",
            Price = 200,
            Description = "Description 2",
            CategoryId = 1,
            Category = null
        };
        await unitOfWork.BookInterface.AddAsync(book);
        await unitOfWork.SaveAsync();

        // Act
        book.Title = "Book 2 Updated";
        unitOfWork.BookInterface.Update(book);
        await unitOfWork.SaveAsync();

        // Assert
        var result = await unitOfWork.BookInterface.GetByIdAsync(book.Id);
        Assert.That(result.Title, Is.EqualTo("Book 2 Updated"));
    }

    [Test]
    public async Task DeleteAsync()
    {
        // Arrange
        var book = new Book
        {
            Id = 3,
            Title = "Book 3",
            Author = "Author 3",
            Price = 300,
            Description = "Description 3",
            CategoryId = 1,
            Category = null
        };

        await unitOfWork.BookInterface.AddAsync(book);
        await unitOfWork.SaveAsync();

        // Act
        unitOfWork.BookInterface.Delete(book.Id);
        await unitOfWork.SaveAsync();

        // Assert
        var result = await unitOfWork.BookInterface.GetByIdAsync(book.Id);
        Assert.IsNull(result);
    }

    [Test]
    public async Task GetAllAsync()
    {
        // Arrange
        var books = await unitOfWork.BookInterface.GetAllAsync();

        // Act
        var count = books.Count();
        
        //Expected
        var expected = dbContext.Books.Count();

        // Assert
        Assert.That(count, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetByIdAsync()
    {
        var book = await unitOfWork.BookInterface.GetByIdAsync(1);

        Assert.IsNotNull(book);
    }
}