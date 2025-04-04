using BLL.DTOs.Requests;
using BLL.DTOs.Responses;
using BLL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryWebApp_v2.Controllers;

[Route("api/books")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService)
    {
        _bookService = bookService;
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("paginated")]
    public async Task<ActionResult<ApiResponse>> GetAllBooksPaginated(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var books = await _bookService.GetAllBooksPaginatedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(new ApiResponse(true, null, books));
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllBooks([FromQuery] string? title, [FromQuery] string? genre, [FromQuery] string? authorName, CancellationToken cancellationToken = default)
    {
        var books = await _bookService.GetAllBooksFilteredAsync(title, genre, authorName, cancellationToken);
        return Ok(books);
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id, CancellationToken cancellationToken = default)
    {
        var book = await _bookService.GetBookByIdAsync(id, cancellationToken);
        return Ok(book);
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("isbn/{isbn}")]
    public async Task<IActionResult> GetBookByISBN(string isbn, CancellationToken cancellationToken = default)
    {
        var book = await _bookService.GetBookByISBNAsync(isbn, cancellationToken);
        return Ok(book);
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpPost]

    public async Task<IActionResult> CreateBook([FromForm] CreateBookDto createBookDto, CancellationToken cancellationToken = default)
    {
        var book = await _bookService.CreateBookAsync(createBookDto, cancellationToken);
        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook([FromForm] UpdateBookDto updateBookDto, CancellationToken cancellationToken = default)
    {
        await _bookService.UpdateBookAsync(updateBookDto, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id, CancellationToken cancellationToken = default)
    {
        await _bookService.DeleteBookAsync(id, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpPost("borrow")]
    public async Task<IActionResult> BorrowBook([FromBody] BorrowBookRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _bookService.BorrowBookAsync(userId, request, cancellationToken);
        return Ok();
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpPost("return/{id}")]
    public async Task<IActionResult> ReturnBook(int id, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _bookService.ReturnBookAsync(userId, id, cancellationToken);
        return Ok();
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("user/rentals")]
    public async Task<IActionResult> GetUserRentals(CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var rentals = await _bookService.GetUserRentalsAsync(userId, cancellationToken);
        return Ok(rentals);
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpPost("{id}/upload-image")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file, CancellationToken cancellationToken = default)
    {
        var imagePath = await _bookService.UploadImageAsync(id, file, cancellationToken);
        return Ok(new { ImagePath = imagePath});
    }
}
