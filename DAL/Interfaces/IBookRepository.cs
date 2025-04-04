using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetAllBooksFilteredAsync(string? title, string? genre, int? authorId, CancellationToken cancellationToken = default);
    Task<Book> GetByISBNAsync(string isbn, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int id, CancellationToken cancellationToken = default);
}
