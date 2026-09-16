namespace QuoteApi.Repositories;
using Microsoft.EntityFrameworkCore;
public interface IRepository<T,TDbContext> where TDbContext:DbContext
{   public Task AddAsync(T item);
    public Task<T?> FindByAsync(Func<T,bool> predicate);
    public Task<List<T>> GetAllAsync();
}