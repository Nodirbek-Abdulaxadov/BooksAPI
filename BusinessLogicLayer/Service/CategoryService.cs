using AutoMapper;
using BooksAPI.DTOs.CategoryDtos;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BusinessLogicLayer.Service;

public class CategoryService : ICategoryService
{
    public readonly IUnitOfWork _unitOfWork;
    public readonly IMapper _mapper;
    public readonly IDistributedCache _cache;
    private const string cacheKey = "categories";

    public CategoryService(IUnitOfWork unitOfWork,
                           IMapper mapper,
                           IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task AddCategoryAsync(AddCategoryDto newCategory)
    {
        var list = await _unitOfWork.CategoryInterface.GetAllAsync();
        var category = _mapper.Map<Category>(newCategory);
        if (newCategory is null)
        {
            throw new CategoryException("Category was null!");
        }

        if (string.IsNullOrEmpty(newCategory.Name))
        {
            throw new CategoryException("Category name is required!");
        }

        if (list.Any(c => c.Name == newCategory.Name))
        {
            throw new CategoryException($"{newCategory.Name} name is already exist!");
        }

        await _unitOfWork.CategoryInterface.AddAsync(category);
        await _unitOfWork.SaveAsync();
        await _cache.RemoveAsync(cacheKey);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.CategoryInterface.GetByIdAsync(id);
        if (category is null)
        {
            throw new CategoryException("Category not found!");
        }
        _unitOfWork.CategoryInterface.Delete(id);
        await _unitOfWork.SaveAsync();
        await _cache.RemoveAsync(cacheKey);
    }

    public async Task<string> GetCategoriesAsync()
    {
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (cachedData is null)
        {
            var list = await _unitOfWork.CategoryInterface.GetAllAsync();
            var categories = list.Select(c => _mapper.Map<CategoryDto>(c))
                             .ToList();
            var json = JsonConvert.SerializeObject(categories);
            await _cache.SetStringAsync(cacheKey, json);
            return json;
        }

        return cachedData;
    }

    public async Task<List<CategoryDto>> GetCategoriesWithBooksAsync()
    {
        var list = await _unitOfWork.CategoryInterface.GetAllCategoriesWithBooksAsync();
        var categories = list.Select(c => _mapper.Map<CategoryDto>(c))
                            .ToList();
        return categories;
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.CategoryInterface.GetByIdAsync(id);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<PagedList<CategoryDto>> GetPagedCategories(int pageSize,
                                                                 int pageNumber)
    {
        var list = await _unitOfWork.CategoryInterface.GetAllAsync();
        var categories = list.Select(c => _mapper.Map<CategoryDto>(c))
                            .ToList();

        PagedList<CategoryDto> pagedList = new(categories,
                                               categories.Count,
                                               pageNumber,
                                               pageSize);
        return pagedList.ToPagedList(categories, pageSize, pageNumber);
    }

    public async Task UpdateCategoryAsync(CategoryDto categoryDto)
    {
        if (categoryDto is null)
        {
            throw new CategoryException("Category was null!");
        }

        if (string.IsNullOrEmpty(categoryDto.Name))
        {
            throw new CategoryException("Category name is required!");
        }
        var category = _mapper.Map<Category>(categoryDto);

        var list = await _unitOfWork.CategoryInterface.GetAllAsync();
        if (list.Any(c => c.Name == categoryDto.Name && c.Id != categoryDto.Id))
        {
            throw new CategoryException($"{categoryDto.Name} name is already exist!");
        }

        _unitOfWork.CategoryInterface.Update(category);
        await _unitOfWork.SaveAsync();
        await _cache.RemoveAsync(cacheKey);
    }
}