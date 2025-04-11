using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class UserBooksRepository : Repository<UserBooks>, IUserBooksRepository
{
    public UserBooksRepository(ApplicationContext context) : base(context) { }

    public async Task<UserBooks?> GetUserRentalAsync(string userId, int bookId, CancellationToken cancellationToken)
    {
        return await _context.UserBooks
            .Include(r => r.Book)
            .Where(r => r.UserId == userId
                && r.BookId == bookId
                && r.Book.IsBorrowed)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserBooks>> GetUserRentalsAsync(string userId, CancellationToken cancellationToken)
    {
        return await _context.UserBooks
            .Include(ub => ub.Book)
                .ThenInclude(b => b.Author)
            .Where(ub => ub.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}
