using QuoteApi.Repositories;
using QuoteApi.Models;
using QuoteApi.Settings;
using Microsoft.Extensions.Options;
namespace QuoteApi.Services;

public interface IQuoteService
{
    public string GenerateRandomQuote();
    public Task<List<Quote>> GetQuotes();
    public IOptions<AuthorSettings> Author_Settings{get;}
    public Task<Quote?> GetQuoteById(int id);
    public Task<Quote?> AddQuoteAsync(string text, string author);
    public event Action? NewQuoteAdded;
    public event Action<Quote>? GetLatestQuote;
    public Task<List<Quote>> GetQuotesWithAuthor();
    public Task ModifyAuthorIdTracked(int authorId, int newId);
    public Task ModifyAuthorIdUntracked(int authorId, int newId);
    public Task<Author?> AddAuthor(string name);
    public Task<Author?> GetAuthorById(int id);
    public Task ModifyAuthorName(int id,string name);
}