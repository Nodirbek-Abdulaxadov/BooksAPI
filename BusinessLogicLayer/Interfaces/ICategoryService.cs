using BooksAPI.DTOs.CategoryDtos;
using BusinessLogicLayer.Helpers;

namespace BusinessLogicLayer.Interfaces;

public interface ICategoryService
{
    Task<PagedList<CategoryDto>> GetPagedCategories(int pageSize, int pageNumber);
    Task<string> GetCategoriesAsync();
    Task<List<CategoryDto>> GetCategoriesWithBooksAsync();
    Task<CategoryDto> GetCategoryByIdAsync(int id);
    Task AddCategoryAsync(AddCategoryDto newCategory);
    Task UpdateCategoryAsync(CategoryDto categoryDto);
    Task DeleteCategoryAsync(int id);
}