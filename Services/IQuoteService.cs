using QuoteApi.Repositories;
using QuoteApi.Models;
namespace QuoteApi.Services;

public interface IQuoteService
{
    public string GenerateRandomQuote();
    public List<Quote> GetQuotes();

    public Quote? GetQuoteById(int id);
    public Quote? AddQuote(string text);
}