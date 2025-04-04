using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(ApplicationContext context) : base(context) { }

    public async Task<Author> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Authors.FirstOrDefaultAsync(a => a.FirstName + " " + a.LastName == name);
    }
}
