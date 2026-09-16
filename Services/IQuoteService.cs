using QuoteApi.Repositories;
using QuoteApi.Models;
namespace QuoteApi.Services;

public interface IQuoteService
{
    public string GenerateRandomQuote();
    public Task<List<Quote>> GetQuotes();

    public Task<Quote?> GetQuoteById(int id);
    public Task<Quote?> AddQuoteAsync(string text);
    public event Action? NewQuoteAdded;
    public event Action<Quote>? GetLatestQuote;
}