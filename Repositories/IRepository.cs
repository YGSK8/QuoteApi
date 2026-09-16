namespace QuoteApi.Repositories;
public interface IRepository<T>
{   public Task AddAsync(T item);
    public Task<T?> FindByAsync(Func<T,bool> predicate);
    public Task<List<T>> GetAllAsync();
}