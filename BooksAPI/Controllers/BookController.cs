using Asp.Versioning;
using BooksAPI.DTOs.BookDtos;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BooksAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
//[Authorize(Roles = "User, Admin")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;


    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var books = await _bookService.GetBooksAsync();
        var model = new Result<BookDto>(true, string.Empty, books);

        var json = JsonConvert.SerializeObject(model, Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

        var ip = HttpContext.Connection.RemoteIpAddress.ToString();
        LoggingService.LogInfo($"User with IP {ip} requested all books");
        return Ok(json);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        var ip = HttpContext.Connection.RemoteIpAddress.ToString();
        LoggingService.LogInfo($"User with IP {ip} requested book with id {id}");
        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> Post(AddBookDto dto)
    {
        var ip = HttpContext.Connection.RemoteIpAddress.ToString();
        try
        {
            await _bookService.AddBookAsync(dto);
            LoggingService.LogInfo($"User with IP {ip} added new book");
            return Ok();
        }
        catch (BookException ex)
        {
            LoggingService.LogError($"User with IP {ip} tried to add new book but got error: {ex.ErrorMessage}");
            return BadRequest(ex.ErrorMessage);
        }
        catch (Exception ex)
        {
            LoggingService.LogError($"User with IP {ip} tried to add new book but got error: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("mock")]
    public async Task<IActionResult> Create5000(AddBookDto dto)
    {
        //for (int i = 0; i < 5000; i++)
        //{
        //    dto.Title = $"Book {i}";
        //    dto.Description = $"Description {i}";
        //    dto.Author = $"Author {i}";
        //    await _bookService.AddBookAsync(dto);
        //}

        //return Ok();

        try
        {
            await _bookService.AddTestAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Put(BookDto book)
    {
        var ip = HttpContext.Connection.RemoteIpAddress.ToString();
        if (ModelState.IsValid)
        {
            await _bookService.UpdateBookAsync(book);
            LoggingService.LogInfo($"User with IP {ip} updated book with id {book.Id}");
            return Ok();
        }

        LoggingService.LogError($"User with IP {ip} tried to update book with id {book.Id} but got error: {ModelState.Values}");
        return BadRequest();
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(int Id)
    {
        await _bookService.DeleteBookAsync(Id);
        var ip = HttpContext.Connection.RemoteIpAddress.ToString();
        LoggingService.LogInfo($"User with IP {ip} deleted book with id {Id}");
        return Ok();
    }

    [HttpGet("filter")]
    public async Task<IActionResult> Filter([FromQuery] FilterParametrs parametrs)
    {
        var books = await _bookService.Filter(parametrs);
        var ip = HttpContext.Connection.RemoteIpAddress.ToString();
        LoggingService.LogInfo($"User with IP {ip} filtered books");
        return Ok(books);
    }
}
