using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(ApplicationContext context) : base(context) { }

    public IQueryable<Book> GetAllBooksQueryable()
    {
        return _context.Books.AsQueryable();
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int id)
    {
        return await _context.Books
            .Where(b => b.AuthorId == id)
            .ToListAsync();
    }

    public async Task<Book?> GetByISBNAsync(string isbn)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.ISBN == isbn);
    }
}
