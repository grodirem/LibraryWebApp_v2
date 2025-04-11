using DAL.Models;

namespace DAL.Interfaces;

public interface IUserBooksRepository : IRepository<UserBooks>
{
    Task<IEnumerable<UserBooks>> GetUserRentalsAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserBooks> GetUserRentalAsync(string userId, int bookId, CancellationToken cancellationToken = default);
}
