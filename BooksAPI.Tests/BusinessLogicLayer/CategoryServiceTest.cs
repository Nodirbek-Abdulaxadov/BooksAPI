namespace BooksAPI.Tests.BusinessLogicLayer;

internal class CategoryServiceTest
{
    private Mock<IUnitOfWork> mockUnitOfWork = new();
    private IUnitOfWork unitOfWork;
    private IMapper mapper;
    private IDistributedCache cache;
    private ICategoryService categoryService;

    [SetUp]
    public void Setup()
    {
        unitOfWork = mockUnitOfWork.Object;
        mapper = (new Mock<IMapper>()).Object;
        cache = (new Mock<IDistributedCache>()).Object;

        categoryService = new CategoryService(unitOfWork, mapper, cache);
    }

    [Test]
    public void AddCategoryAsync_WhenCategoryIsNull_ThrowsCategoryException()
    {
        // Arrange
        AddCategoryDto dto = null;
        
        mockUnitOfWork.Setup(u => u.CategoryInterface.AddAsync(It.IsAny<Category>()))
                      .Verifiable();
        mockUnitOfWork.Setup(u => u.SaveAsync())
                      .Verifiable();

        //Act & Assert
        var ex = Assert.ThrowsAsync<CategoryException>(() => categoryService.AddCategoryAsync(dto));
        Assert.That(ex.ErrorMessage, Is.EqualTo("Category was null!"));
    }

    [Test]
    public void AddCategoryAsync_WhenCategoryNameIsEmptyOrNull_ThrowsCategoryException()
    {
        // Arrange
        AddCategoryDto dto = new()
        {
            Name = ""
        };

        mockUnitOfWork.Setup(u => u.CategoryInterface.AddAsync(It.IsAny<Category>()))
                      .Verifiable();
        mockUnitOfWork.Setup(u => u.SaveAsync())
                      .Verifiable();

        //Act & Assert
        Assert.ThrowsAsync<CategoryException>(() => categoryService.AddCategoryAsync(dto));
    }


    [Test]
    public void AddCategoryAsync_WhenCategoryNameIsAlreadyExist_ThrowsCategoryException()
    {
        mockUnitOfWork.Setup(u => u.CategoryInterface.GetAllAsync())
                      .ReturnsAsync(new List<Category>
                      {
                          new Category
                          {
                              Name = "Category1"
                          }
                      });

        mockUnitOfWork.Setup(u => u.CategoryInterface.AddAsync(It.IsAny<Category>()))
                      .Verifiable();
        mockUnitOfWork.Setup(u => u.SaveAsync())
                      .Verifiable();
        // Arrange
        AddCategoryDto dto = new()
        {
            Name = "Category1"
        };

        //Act & Assert
        Assert.ThrowsAsync<CategoryException>(() => categoryService.AddCategoryAsync(dto));
    }

    [Test]
    public async Task AddCategoryAsync_WhenCategoryIsValid()
    {
        // Arrange
        AddCategoryDto dto = new()
        {
            Name = "Category1"
        };

        mockUnitOfWork.Setup(u => u.CategoryInterface.GetAllAsync())
                      .ReturnsAsync(new List<Category>());

        mockUnitOfWork.Setup(u => u.CategoryInterface.AddAsync(It.IsAny<Category>()))
                      .Verifiable();
        mockUnitOfWork.Setup(u => u.SaveAsync())
                      .Verifiable();

        //Act
        await categoryService.AddCategoryAsync(dto);

        //Assert
        mockUnitOfWork.Verify(u => u.CategoryInterface.AddAsync(It.IsAny<Category>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once); 
    }

    [Test]
    public void DeleteAsync_WhenCategoryNotExists_ShouldThrowCategoryException()
    {
        Category nullCategory = null;
        mockUnitOfWork.Setup(u => u.CategoryInterface.GetByIdAsync(It.IsAny<int>()))
                      .ReturnsAsync(nullCategory);

        //Act & Assert
        var ex = Assert.ThrowsAsync<CategoryException>(() => categoryService.DeleteCategoryAsync(1));
        Assert.That(ex.ErrorMessage, Is.EqualTo("Category not found!"));
    }

    [Test]
    public async Task DeleteAsync_WhenCategoryExists_ShouldDeleteCategory()
    {
        mockUnitOfWork.Setup(u => u.CategoryInterface.GetByIdAsync(It.IsAny<int>()))
                      .ReturnsAsync(new Category());

        mockUnitOfWork.Setup(u => u.CategoryInterface.Delete(It.IsAny<int>()))
            .Verifiable();

        mockUnitOfWork.Setup(u => u.SaveAsync())
                      .Verifiable();
        //Act
        await categoryService.DeleteCategoryAsync(5);

        //Assert
        mockUnitOfWork.Verify(u => u.CategoryInterface.Delete(It.IsAny<int>()), Times.Once);
    }

    /*[Test] Cache problem
    public async Task GetCategoriesAsync_ShouldReturnAllCategories()
    {
        var mockCache = new Mock<IDistributedCache>();
        mockCache.Setup(c => c.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((string)null);

        mockUnitOfWork.Setup(u => u.CategoryInterface.GetAllAsync())
                      .ReturnsAsync(new List<Category>
                      {
                          new Category
                          {
                              Name = "Category"
                          },
                          new Category
                          {
                              Name = "Category"
                          }
                      });

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>()))
                  .Returns(new CategoryDto() { Name = "Category"});

        var categoryService = new CategoryService(mockUnitOfWork.Object, mockMapper.Object, mockCache.Object);

        //Act
        var result = await categoryService.GetCategoriesAsync();

        //Assert
        Assert.That(result, Is.Not.Null);
    }*/

    [Test]
    public async Task GetCategoriesWithBooks_ShouldReturnAllCategoriesWithTheirBooks()
    {
        mockUnitOfWork.Setup(u => u.CategoryInterface.GetAllCategoriesWithBooksAsync())
                      .ReturnsAsync(new List<Category>
                      {
                          new Category
                          {
                              Name = "Category",
                              Books = new List<Book>
                              {
                                  new Book
                                  {
                                      Title = "Book1"
                                  }
                              }
                          }
                      })
                      .Verifiable();

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>()))
                  .Returns(new CategoryDto() { Name = "Category" })
                  .Verifiable();

        var categoryService = new CategoryService(mockUnitOfWork.Object, mockMapper.Object, cache);

        //Act
        var result = await categoryService.GetCategoriesWithBooksAsync();

        //Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetCategoryByIdAsync_WhenCategoryExists_ShouldReturnCategory()
    {
        mockUnitOfWork.Setup(u => u.CategoryInterface.GetByIdAsync(It.IsAny<int>()))
                      .ReturnsAsync(new Category
                      {
                          Name = "Category"
                      })
                      .Verifiable();

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>()))
                  .Returns(new CategoryDto() { Name = "Category" })
                  .Verifiable();

        var categoryService = new CategoryService(mockUnitOfWork.Object, mockMapper.Object, cache);

        //Act
        var result = await categoryService.GetCategoryByIdAsync(1);

        //Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetPagedCategories_ShouldReturnPagedCategories()
    {
        mockUnitOfWork.Setup(u => u.CategoryInterface.GetAllAsync())
                      .ReturnsAsync(new List<Category>
                      {
                          new Category
                          {
                              Name = "Category"
                          }
                      })
                      .Verifiable();

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>()))
                  .Returns(new CategoryDto() { Name = "Category" })
                  .Verifiable();

        var categoryService = new CategoryService(mockUnitOfWork.Object, mockMapper.Object, cache);

        //Act
        var result = await categoryService.GetPagedCategories(5, 1);

        //Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void UpdateCategoryAsync_WhenCategoryIsNull_ThrowsCategoryException()
    {
        // Arrange
        CategoryDto dto = null;

        //Act & Assert
        var ex = Assert.ThrowsAsync<CategoryException>(() => categoryService.UpdateCategoryAsync(dto));
        Assert.That(ex.ErrorMessage, Is.EqualTo("Category was null!"));
    }

    [Test]
    public void UpdateCategoryAsync_WhenCategoryNameIsEmptyOrNull_ThrowsCategoryException()
    {
        // Arrange
        CategoryDto dto = new()
        {
            Name = ""
        };

        //Act & Assert
        var ex = Assert.ThrowsAsync<CategoryException>(() => categoryService.UpdateCategoryAsync(dto));
        Assert.That(ex.ErrorMessage, Is.EqualTo("Category name is required!"));
    }

    [Test]
    public async Task UpdateCategoryAsync_WhenCategoryIsValid()
    {
        mockUnitOfWork.Setup(u => u.CategoryInterface.GetAllAsync())
                      .ReturnsAsync(new List<Category>())
                      .Verifiable();

        mockUnitOfWork.Setup(u => u.CategoryInterface.Update(It.IsAny<Category>()))
                      .Verifiable();

        mockUnitOfWork.Setup(u => u.SaveAsync())
                      .Verifiable();

        // Arrange
        CategoryDto dto = new()
        {
            Name = "Category1"
        };

        //Act
        await categoryService.UpdateCategoryAsync(dto);

        //Assert
        mockUnitOfWork.Verify(u => u.CategoryInterface.Update(It.IsAny<Category>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveAsync());
    }
}