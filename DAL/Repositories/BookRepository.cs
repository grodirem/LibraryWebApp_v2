using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(ApplicationContext context) : base(context) { }

    public async Task<IEnumerable<Book>> GetAllBooksFilteredAsync(string? title, string? genre, int? authorId, CancellationToken cancellationToken = default)
    {
        var booksQuery = _context.Books
            .Include(b => b.Author)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            booksQuery = booksQuery.Where(b => b.Title.Contains(title));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            booksQuery = booksQuery.Where(b => b.Genre ==  genre);
        }

        if (authorId.HasValue)
        {
            booksQuery = booksQuery.Where(b => b.AuthorId == authorId);
        }

        return await booksQuery.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Books
            .Where(b => b.AuthorId == id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Book?> GetByISBNAsync(string isbn, CancellationToken cancellationToken = default)
    {
        return await _context.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.ISBN == isbn, cancellationToken);
    }

    public async Task<Book?> GetByIdWithAuthorAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}
