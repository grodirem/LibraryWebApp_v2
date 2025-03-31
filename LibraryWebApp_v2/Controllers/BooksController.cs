using BLL.DTOs.Requests;
using BLL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryWebApp_v2.Controllers;

[AllowAnonymous]
[Route("api/books")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService)
    {
        _bookService = bookService;
    }

    // не работает
    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllBooks([FromQuery] string search, [FromQuery] string genre, [FromQuery] string author)
    {
        var books = await _bookService.GetAllBooksAsync(search, genre, author);
        return Ok(books);
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("isbn/{isbn}")]
    public async Task<IActionResult> GetBookByISBN(string isbn)
    {
        var book = await _bookService.GetBookByISBNAsync(isbn);

        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpPost]

    public async Task<IActionResult> CreateBook([FromForm] CreateBookDto createBookDto)
    {
        var book = await _bookService.CreateBookAsync(createBookDto);
        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook([FromForm] UpdateBookDto updateBookDto)
    {
        await _bookService.UpdateBookAsync(updateBookDto);
        return NoContent();
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        await _bookService.DeleteBookAsync(id);
        return NoContent();
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpPost("borrow")]
    public async Task<IActionResult> BorrowBook([FromForm] BorrowBookRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Пользователь не авторизован.");
        }

        await _bookService.BorrowBookAsync(userId, request);
        return Ok();
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpPost("return/{id}")]
    public async Task<IActionResult> ReturnBook(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Пользователь не авторизован.");
        }

        await _bookService.ReturnBookAsync(userId, id);
        return Ok();
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("user/rentals")]
    public async Task<IActionResult> GetUserRentals()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Пользователь не авторизован.");
        }

        var rentals = await _bookService.GetUserRentalsAsync(userId);
        return Ok(rentals);
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpPost("{id}/upload-image")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        var imagePath = await _bookService.UploadImageAsync(id, file);
        return Ok(new { ImagePath = imagePath});
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("paginated")]
    public async Task<ActionResult<ApiResponse>> GetAllBooksPaginated(int pageIndex = 1, int pageSize = 10)
    {
        var books = await _bookService.GetAllBooksPaginatedAsync(pageIndex, pageSize);
        return new ApiResponse(true, null, books);
    }
}
