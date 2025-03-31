using DAL.Models;

namespace DAL.Interfaces;

public interface IAuthorRepository : IRepository<Author>
{
    Task<Author> GetByNameAsync(string name);
}
