using BooksAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BooksAPI.Tests.PresentationLayer;

internal class CategoryControllerTest
{
    Mock<ICategoryService> _categoryServiceMock;
    CategoryController controller;

    [SetUp]
    public void Setup()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        controller = new CategoryController(_categoryServiceMock.Object);
    }

    [Test]
    public async Task Get_ShouldReturnCategoryDtosListWithOk()
    {
        //Arrange
        var categories = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Name = "Category 1" },
            new CategoryDto { Id = 2, Name = "Category 2" }
        };
        var json = JsonConvert.SerializeObject(categories);

        _categoryServiceMock.Setup(x => x.GetCategoriesAsync())
                            .ReturnsAsync(json)
                            .Verifiable();

        // Act
        var result = await controller.Get();

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);

        var okResult = result as OkObjectResult;
        Assert.IsInstanceOf<string>(okResult.Value);
    }

    [Test]
    public async Task GetWithBooks_ShouldReturnCategoryDtosListWithOk()
    {
        //Arrange
        var categories = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Name = "Category 1" },
            new CategoryDto { Id = 2, Name = "Category 2" }
        };

        _categoryServiceMock.Setup(x => x.GetCategoriesWithBooksAsync())
                            .ReturnsAsync(categories)
                            .Verifiable();

        // Act
        var result = await controller.GetWithBooks();

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);

        var okResult = result as OkObjectResult;
        Assert.IsInstanceOf<string>(okResult.Value);
    }

    [Test]
    public async Task Get_ShouldReturnCategoryDtoWithOk()
    {
        //Arrange
        var category = new CategoryDto { Id = 1, Name = "Category 1" };

        _categoryServiceMock.Setup(x => x.GetCategoryByIdAsync(1))
                            .ReturnsAsync(category)
                            .Verifiable();

        // Act
        var result = await controller.Get(1);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);

        var okResult = result as OkObjectResult;
        Assert.IsInstanceOf<CategoryDto>(okResult.Value);
    }

    [Test]
    public async Task Post_WhenCategoryNotValid_ShoultReturnBadRequest()
    {
        var dto = new AddCategoryDto() { Name = "" };
        _categoryServiceMock.Setup(x => x.AddCategoryAsync(dto))
                            .ThrowsAsync(new CategoryException("Name is required"))
                            .Verifiable();

        // Act
        var result = await controller.Post(dto);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task Post_ShouldReturnOk()
    {
        //Arrange
        var dto = new AddCategoryDto { Name = "Category 1" };

        // Act
        var result = await controller.Post(dto);

        // Assert
        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public async Task Post_WhenExceptionIsThrown_ShouldReturnStatusCode500()
    {
        //Arrange
        var dto = new AddCategoryDto { Name = "Category 1" };

        _categoryServiceMock.Setup(x => x.AddCategoryAsync(dto))
                            .ThrowsAsync(new Exception("Error while adding category"))
                            .Verifiable();

        // Act
        var result = await controller.Post(dto);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task Put_ShouldReturnOK()
    {
        //Arrange
        var category = new CategoryDto { Id = 1, Name = "Category 1" };

        // Act
        var result = await controller.Put(category);

        // Assert
        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public async Task Put_WhenCategoryNotValid_ShoultReturnBadRequest()
    {
        var category = new CategoryDto() { Id = 1, Name = "" };
        _categoryServiceMock.Setup(x => x.UpdateCategoryAsync(category))
                            .ThrowsAsync(new CategoryException("Name is required"))
                            .Verifiable();

        // Act
        var result = await controller.Put(category);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
    }

    [Test]
    public async Task Put_WhenExceptionIsThrown_ShouldReturnStatusCode500()
    {
        //Arrange
        var category = new CategoryDto { Id = 1, Name = "Category 1" };

        _categoryServiceMock.Setup(x => x.UpdateCategoryAsync(category))
                            .ThrowsAsync(new Exception("Error while updating category"))
                            .Verifiable();

        // Act
        var result = await controller.Put(category);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task Delete_ShouldReturnOK()
    {
        // Act
        var result = await controller.Delete(1);

        // Assert
        Assert.IsInstanceOf<OkResult>(result);
    }
}