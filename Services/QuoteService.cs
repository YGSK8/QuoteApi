using QuoteApi.Repositories;
using QuoteApi.Models;
using QuoteApi.Data;
using System.Linq.Expressions;

namespace QuoteApi.Services;

public class QuoteService:IQuoteService
{
    private List<string> _quotes = ["String 1", "String 2", "String 3", "String 4", "String 5", "String 6","String 7","String 8","String 9","String 10"];
    private IRepository<Quote,QuoteApiDbContext> _quoteRepository;
    private IRepository<Author,QuoteApiDbContext> _authorRepository;
    public event Action? NewQuoteAdded;
    public event Action<Quote>? GetLatestQuote;
    public QuoteService(IRepository<Quote,QuoteApiDbContext> quoteRepository, IRepository<Author,QuoteApiDbContext> authorRepository)
    {
        _quoteRepository = quoteRepository;
        _authorRepository = authorRepository;
    }
    public string GenerateRandomQuote()
    {
        Random rand = new Random();
        return _quotes[rand.Next(0,10)];
    }
    public async Task<List<Quote>> GetQuotes()
    {
        return await _quoteRepository.GetAllAsync();
    }
    
    public async Task<Quote?> AddQuoteAsync(string text, string name)
    {
        Expression<Func<Quote,bool>> expression = (quote)=>quote.Text==text;
        if(await _quoteRepository.FindByAsync(expression) == null)
        {
            Expression<Func<Author,bool>> expression1 = (author)=>author.Name==name;
            Author? author = await _authorRepository.FindByAsync(expression1);
            if(author == null)
            {
                author = new Author(name){Name = name};
                await _authorRepository.AddAsync(author);
            }
            await _quoteRepository.AddAsync(new Quote(text,author.Id));
            return await _quoteRepository.FindByAsync(expression);
        }
        return null;
    }
    public async Task<Quote?> GetQuoteById(int id)
    {
        Expression<Func<Quote,bool>> expression = quote => quote.Id==id;
        return await _quoteRepository.FindByAsync(expression);
    }
}