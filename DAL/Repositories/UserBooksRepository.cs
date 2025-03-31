using DAL.Models;

namespace DAL.Repositories;

public class UserBooksRepository : Repository<UserBooks>
{
    public UserBooksRepository(ApplicationContext context) : base(context) { }
}
