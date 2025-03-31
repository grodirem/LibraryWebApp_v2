using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebApp_v2.Controllers;

[AllowAnonymous]
[Route("api/authors")]
[ApiController]
public class AuthorsController : ControllerBase
{
    private readonly AuthorService _authorService;

    public AuthorsController(AuthorService authorService)
    {
        _authorService = authorService;
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet]
    public async Task<IActionResult> GetAllAuthors()
    {
        var authors = await _authorService.GetAllAuthorsAsync();
        return Ok(authors);
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthorById(int id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);

        if (author == null)
        {
            return NotFound();
        }

        return Ok(author);
    }

    [Authorize(Policy = "AuthenticatedUsers")]
    [HttpGet("{id}/books")]
    public async Task<IActionResult> GetBooksByAuthorId(int id)
    {
        var books = await _authorService.GetBooksByAuthorIdAsync(id);
        return Ok(books);
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpPost]
    public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorDto createAuthorDto)
    {
        var author = await _authorService.CreateAuthorAsync(createAuthorDto);
        return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, author);
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor([FromForm] UpdateAuthorDto updateAuthorDto)
    {
        await _authorService.UpdateAuthorAsync(updateAuthorDto);
        return NoContent();
    }

    [Authorize(Policy = "OnlyAdminUsers")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        await _authorService.DeleteAuthorAsync(id);
        return NoContent();
    }
}
