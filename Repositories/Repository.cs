namespace QuoteApi.Repositories;
using QuoteApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class Repository<T,TDbContext>:IRepository<T,TDbContext> where TDbContext:DbContext where T:class
{
    private TDbContext _dbContext;

    public Repository(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(T item)
    {
        _dbContext.Add(item);
        int count = await _dbContext.SaveChangesAsync();
        Console.WriteLine($"successfully uploaded {count} record/s");
    }
    public async Task<T?> FindByAsync(Expression<Func<T,bool>> predicate)
    {
        return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
    }
    public async Task<List<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync<T>();
        
    }
}