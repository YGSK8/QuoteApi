namespace QuoteApi.Repositories;
public interface IRepository<T>
{   public void Add(T item);
    public T? FindBy(Func<T,bool> predicate);
    public List<T> GetAll();
}