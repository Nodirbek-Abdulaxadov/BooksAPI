namespace BooksAPI.Tests.DataAccessLayer;

internal class CategoryRepositoryTest
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
        var category = new Category() { Name = "Test" };
        await unitOfWork.CategoryInterface.AddAsync(category);
        await unitOfWork.SaveAsync();

        // Act
        var result = dbContext.Categories.FirstOrDefault(c => c.Name == "Test");

        // Assert
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task GetAllAsync()
    {
        // Arrange
        var categories = await unitOfWork.CategoryInterface.GetAllAsync();

        // Act
        var result = categories.Count();

        //Expected
        var expectedCount = dbContext.Categories.Count();

        // Assert
        Assert.That(result, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task GetByIdAsync()
    {
        // Arrange
        var category = new Category() { Id = 2, Name = "Test" };
        await unitOfWork.CategoryInterface.AddAsync(category);
        await unitOfWork.SaveAsync();

        // Act
        var result = await unitOfWork.CategoryInterface.GetByIdAsync(category.Id);

        // Assert
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task Update()
    {
        var category = new Category() { Id = 3, Name = "Test3" };
        await unitOfWork.CategoryInterface.AddAsync(category);
        await unitOfWork.SaveAsync();

        // Act
        category.Name = "Test3 Updated";
        unitOfWork.CategoryInterface.Update(category);
        await unitOfWork.SaveAsync();

        var result = await unitOfWork.CategoryInterface.GetByIdAsync(category.Id);
        Assert.That(result.Name, Is.EqualTo("Test3 Updated"));
    }

    [Test]
    public async Task Delete()
    {
        // Arrange
        var category = new Category() { Id = 4, Name = "Test4" };
        await unitOfWork.CategoryInterface.AddAsync(category);
        await unitOfWork.SaveAsync();

        // Act
        unitOfWork.CategoryInterface.Delete(category.Id);
        await unitOfWork.SaveAsync();

        //Assert
        var result = await unitOfWork.CategoryInterface.GetByIdAsync(category.Id);
        Assert.IsNull(result);
    }
}