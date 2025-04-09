using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
namespace DAL.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet;
    protected readonly ApplicationContext _context;

    public Repository(ApplicationContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await SaveAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await SaveAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await SaveAsync(cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PaginatedList<T>> GetAllPaginated(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await _dbSet.CountAsync(cancellationToken);
        
        var items = await _dbSet
            .OrderBy(i => EF.Property<object>(i, "Id"))
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        return new PaginatedList<T>(items, pageIndex, totalPages);
    }
}
