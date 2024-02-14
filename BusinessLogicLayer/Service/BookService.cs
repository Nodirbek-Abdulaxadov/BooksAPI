using AutoMapper;
using BooksAPI.DTOs.BookDtos;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Service;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly AppDbContext dbContext;

    public BookService(IUnitOfWork unitOfWork,
                       IMapper mapper,
                       AppDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        this.dbContext = dbContext;
    }

    public async Task AddBookAsync(AddBookDto newBook)
    {
        if (newBook == null)
        {
            throw new BookException("Book was null");
        }

        if (string.IsNullOrEmpty(newBook.Title))
        {
            throw new BookException("Book title is required!");
        }

        var books = await _unitOfWork.BookInterface.GetAllAsync();
        if (books.Any(b => b.Title == newBook.Title))
        {
            throw new BookException($"{newBook.Title} is already exist!");
        }

        if (newBook.Price <= 0)
        {
            throw new BookException("Price must be non negative!");
        }

        var category = await _unitOfWork.CategoryInterface.GetByIdAsync(newBook.CategoryId);
        if (category == null)
        {
            throw new BookException("Category doesn't exist!");
        }

        var book = _mapper.Map<Book>(newBook);
        book.Category = null;
        await _unitOfWork.BookInterface.AddAsync(book);
        await _unitOfWork.SaveAsync();
    }

    public async Task AddTestAsync()
    {
        var tags = await dbContext.Tags.ToListAsync();
        var book = new Book
        {
            Title = "Test",
            Price = 100,
            CategoryId = 1,
            Category = null,
            Tags = tags
        };

        await dbContext.Books.AddAsync(book);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(int id)
    {
        _unitOfWork.BookInterface.Delete(id);
        await _unitOfWork.SaveAsync();
    }

    public async Task<PagedList<BookDto>> Filter(FilterParametrs parametrs)
    {
        var list = await _unitOfWork.BookInterface.GetAllAsync();
        // Filter by title
        if (parametrs.Title is not "")
        {
            list = list.Where(book => book.Title.ToLower()
                .Contains(parametrs.Title.ToLower()));
        }

        // Filter by price
        list = list.Where(book => book.Price >= parametrs.MinPrice &&
                                          book.Price <= parametrs.MaxPrice);

        var dtos = list.Select(book => _mapper.Map<BookDto>(book)).ToList();
        // Order by title
        if (parametrs.OrderByTitle)
        {
            dtos = dtos.OrderBy(book => book.Title).ToList();
        }
        else
        {
            dtos = dtos.OrderByDescending(book => book.Price).ToList();
        }

        PagedList<BookDto> pagedList = new(dtos, dtos.Count, 
                                          parametrs.PageNumber, parametrs.PageSize);

        return pagedList.ToPagedList(dtos, parametrs.PageSize, parametrs.PageNumber);
    }

    public async Task<BookDto> GetBookByIdAsync(int id)
    {
        var book = await _unitOfWork.BookInterface.GetByIdAsync(id);
        return _mapper.Map<BookDto>(book);
    }

    public async Task<List<BookDto>> GetBooksAsync()
    {
        var list = await _unitOfWork.BookInterface.GetBooksWithCategoryAsync();
        return list.Select(book => _mapper.Map<BookDto>(book)).ToList();
    }

    public async Task UpdateBookAsync(BookDto BookDto)
    {
        var book = _mapper.Map<Book>(BookDto);
        _unitOfWork.BookInterface.Update(book);
        await _unitOfWork.SaveAsync();
    }
}
