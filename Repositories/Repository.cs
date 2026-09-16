namespace QuoteApi.Repositories;
using QuoteApi.Data;
using Microsoft.EntityFrameworkCore;
public class Repository<T>:IRepository<T>
{
    private List<T> _items = new ();
    public Task AddAsync(T item)
    {
        _items.Add(item);
        return Task.CompletedTask;
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