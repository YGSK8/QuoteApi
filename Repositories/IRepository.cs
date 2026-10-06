namespace QuoteApi.Repositories;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
public interface IRepository<T,TDbContext> where TDbContext:DbContext
{   public Task AddAsync(T item);
    public Task<T?> FindByAsync(Expression<Func<T,bool>> predicate);
    public Task<List<T>> GetAllAsync();

    public  Task<List<T>> GetAllWithIncludeAsync<TProperty>(Expression<Func<T,TProperty>> predicate);
    public Task<List<T>> GetTUsingWhereTracked(Expression<Func<T,bool>> expression);
    public Task<List<T>> GetTUsingWhereUntracked(Expression<Func<T,bool>> expression);
    public Task SaveChanges();
    public Task<IDbContextTransaction> BeginTransactionAsync();
}