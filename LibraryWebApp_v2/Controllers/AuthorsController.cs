using BLL.DTOs.Requests;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebApp_v2.Controllers;

[Route("api/authors")]
[ApiController]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet]
    public async Task<IActionResult> GetAllAuthors(CancellationToken cancellationToken = default)
    {
        var authors = await _authorService.GetAllAuthorsAsync(cancellationToken);
        return Ok(authors);
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthorById(int id, CancellationToken cancellationToken = default)
    {
        var author = await _authorService.GetAuthorByIdAsync(id, cancellationToken);
        return Ok(author);
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("{id}/books")]
    public async Task<IActionResult> GetBooksByAuthorId(int id, CancellationToken cancellationToken = default)
    {
        var books = await _authorService.GetBooksByAuthorIdAsync(id, cancellationToken);
        return Ok(books);
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpPost]
    public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorDto createAuthorDto, CancellationToken cancellationToken = default)
    {
        var author = await _authorService.CreateAuthorAsync(createAuthorDto, cancellationToken);
        return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, author);
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor([FromForm] UpdateAuthorDto updateAuthorDto, CancellationToken cancellationToken = default)
    {
        await _authorService.UpdateAuthorAsync(updateAuthorDto, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id, CancellationToken cancellationToken = default)
    {
        await _authorService.DeleteAuthorAsync(id, cancellationToken);
        return NoContent();
    }
}
