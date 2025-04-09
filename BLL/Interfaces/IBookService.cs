using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using DAL.Models;
using Microsoft.AspNetCore.Http;

namespace BLL.Interfaces;

public interface IBookService
{
    Task<BookDto> GetBookByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PaginatedList<Book>> GetAllBooksPaginatedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<BookDto> CreateBookAsync(CreateBookDto createDto, CancellationToken cancellationToken = default);
    Task UpdateBookAsync(UpdateBookDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteBookAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookDto>> GetAllBooksFilteredAsync(string? title, string? genre, string? authorName, CancellationToken cancellationToken = default);
    Task<BookDto> GetBookByISBNAsync(string isbn, CancellationToken cancellationToken = default);
    Task<string> UploadImageAsync(int id, IFormFile file, CancellationToken cancellationToken = default);
    Task BorrowBookAsync(string userId, BorrowBookRequest request, CancellationToken cancellationToken = default);
    Task ReturnBookAsync(string userId, int bookId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserBooksDto>> GetUserRentalsAsync(string userId, CancellationToken cancellationToken = default);
}
