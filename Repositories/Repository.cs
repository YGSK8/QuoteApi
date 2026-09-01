namespace QuoteApi.Repositories;

public class Repository<T>:IRepository<T>
{
    private List<T> _items = new ();
    public void Add(T item)
    {
        _items.Add(item);
    }
    public T? FindBy(Func<T,bool> predicate)
    {
        foreach(T item in _items)
        {
            if (predicate(item))return item;
        }
        return default;
    }
    public List<T> GetAll()
    {
        return [.._items];
    }
}