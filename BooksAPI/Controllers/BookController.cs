using BooksAPI.Data;
using BooksAPI.Models;
using Microsoft.AspNetCore.Mvc;
namespace BooksAPI.Controllers;

[ApiController]
[Route("api/book")]
public class BookController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public BookController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("getall")]
    public IActionResult GetAll()
    {
        var lisOfBooks = _dbContext.Books.ToList();
        return Ok(lisOfBooks);
    }

    [HttpGet]
    [Route("get/{id}")]
    public IActionResult Get(int id)
    {
        var book = _dbContext.Books.FirstOrDefault(book => book.Id == id);
        return Ok(book);
    }

    [HttpPost]
    [Route("add")]
    public IActionResult Add(Book book)
    {
        _dbContext.Books.Add(book);
        _dbContext.SaveChanges();

        return Ok(book);
    }
    
    [HttpPut]
    [Route("update")]
    public IActionResult Update(Book book)
    {
        _dbContext.Update(book);
        _dbContext.SaveChanges();
        return Ok(book);
    }

    [HttpDelete]
    [Route("delete/{id}")]
    public IActionResult Delete(int id)
    {
        var book = _dbContext.Books.FirstOrDefault(b => b.Id == id);
        _dbContext.Remove(book);
        _dbContext.SaveChanges();
        return Ok();
    }
}

