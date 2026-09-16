using QuoteApi.Repositories;
using QuoteApi.Models;
using QuoteApi.Data;

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
    public async Task<List<Quote>> GetQuotes()
    {
        return await _repository.GetAllAsync();
    }
    
    public async Task<Quote?> AddQuoteAsync(string text)
    {
        if(await _repository.FindByAsync((quote)=>{if(quote.Text==text)return true;return false;}) == null)
        {
            int id = (await _repository.GetAllAsync()).Count+1;
            Quote quote = new(id,text);
            await _repository.AddAsync(quote);
            NewQuoteAdded?.Invoke();
            GetLatestQuote?.Invoke(quote);
            return quote;
        }
        return null;
    }
    public async Task<Quote?> GetQuoteById(int id)
    {
        return await _repository.FindByAsync(quote => quote.Id==id);
    }
}