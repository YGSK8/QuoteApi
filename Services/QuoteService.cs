using QuoteApi.Repositories;
using QuoteApi.Models;

namespace QuoteApi.Services;

public class QuoteService:IQuoteService
{
    private List<string> _quotes = ["String 1", "String 2", "String 3", "String 4", "String 5", "String 6","String 7","String 8","String 9","String 10"];
    private IRepository<Quote> _repository;
    public event Action? NewQuoteAdded;
    public event Action<Quote>? GetLatestQuote;
    public QuoteService(IRepository<Quote> repository)
    {
        _repository = repository;
    }
    public string GenerateRandomQuote()
    {
        Random rand = new Random();
        return _quotes[rand.Next(0,10)];
    }
    public List<Quote> GetQuotes()
    {
        return _repository.GetAll();
    }
    
    public Quote? AddQuote(string text)
    {
        if(_repository.FindBy((quote)=>{if(quote.Text==text)return true;return false;}) == null)
        {
            int id = _repository.GetAll().Count+1;
            Quote quote = new(id,text);
            _repository.Add(quote);
            NewQuoteAdded?.Invoke();
            GetLatestQuote?.Invoke(quote);
            return quote;
        }
        return null;
    }
    public Quote? GetQuoteById(int id)
    {
        return _repository.FindBy(quote => quote.Id==id);
    }
}