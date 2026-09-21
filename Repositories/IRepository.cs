namespace QuoteApi.Repositories;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
public interface IRepository<T,TDbContext> where TDbContext:DbContext
{   public Task AddAsync(T item);
    public Task<T?> FindByAsync(Expression<Func<T,bool>> predicate);
    public Task<List<T>> GetAllAsync();
}