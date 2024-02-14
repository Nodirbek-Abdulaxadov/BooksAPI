using Asp.Versioning;
using BooksAPI.DTOs.CategoryDtos;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace BooksAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "User, Admin")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var categories = await _categoryService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("withbooks")]
    public async Task<IActionResult> GetWithBooks()
    {
        var categories = await _categoryService.GetCategoriesWithBooksAsync();
        var json = JsonConvert.SerializeObject(categories, Formatting.Indented,
                       new JsonSerializerSettings
                       {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        return Ok(json);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Post(AddCategoryDto dto)
    {
        try
        {
            await _categoryService.AddCategoryAsync(dto);
            Log.Information($"Category {dto.Name} added");
            return Ok();
        }
        catch(CategoryException ex)
        {
            Log.Error(ex, "Error while adding category");
            return BadRequest(ex.ErrorMessage);
        }
        catch(Exception ex)
        {
            Log.Error(ex, "Error while adding category");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Put(CategoryDto category)
    {
        try
        {
            await _categoryService.UpdateCategoryAsync(category);
            return Ok();
        }
        catch (CategoryException ex)
        {
            return BadRequest(ex.ErrorMessage);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(int Id)
    {
        await _categoryService.DeleteCategoryAsync(Id);
        return Ok();
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(int pageSize = 10, int pageNumber = 1)
    {
        var paged = await _categoryService.GetPagedCategories(pageSize, pageNumber);

        var metaData = new
        {
            paged.TotalCount,
            paged.PageSize,
            paged.CurrentPage,
            paged.HasNext,
            paged.HasPrevious
        };

        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

        return Ok(paged.Data);
    }
}