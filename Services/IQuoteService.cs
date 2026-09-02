using QuoteApi.Repositories;
using QuoteApi.Models;
namespace QuoteApi.Services;

public interface IQuoteService
{
    public string GenerateRandomQuote();
    public List<Quote> GetQuotes();

    public Quote? GetQuoteById(int id);
    public Quote? AddQuote(string text);
    public event Action? NewQuoteAdded;
    public event Action<Quote>? GetLatestQuote;
}