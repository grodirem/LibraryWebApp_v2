using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using DAL.Models;

namespace BLL.Interfaces;

public interface IAuthorService
{
    Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync(CancellationToken cancellationToken = default);
    Task<AuthorDto> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto, CancellationToken cancellationToken = default);
    Task UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto, CancellationToken cancellationToken = default);
    Task DeleteAuthorAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int id, CancellationToken cancellationToken = default);
}
