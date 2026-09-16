namespace QuoteApi.Repositories;
using QuoteApi.Data;
using Microsoft.EntityFrameworkCore;
public class Repository<T,TDbContext>:IRepository<T,TDbContext> where TDbContext:DbContext
{
    private List<T> _items = new ();
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
    public Task<T?> FindByAsync(Func<T,bool> predicate)
    {
        foreach(T item in _items)
        {
            if (predicate(item))return Task.FromResult<T?>(item); 
        }
        return Task.FromResult<T?>(default);
    }
    public Task<List<T>> GetAllAsync()
    {
        return Task.FromResult<List<T>>([.._items]);
    }
}