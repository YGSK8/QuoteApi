using QuoteApi.Repositories;
using QuoteApi.Models;
using QuoteApi.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace QuoteApi.Services;

public class QuoteService:IQuoteService
{
    private List<string> _quotes = ["String 1", "String 2", "String 3", "String 4", "String 5", "String 6","String 7","String 8","String 9","String 10"];
    private IRepository<Quote,QuoteApiDbContext> _quoteRepository;
    private IRepository<Author,QuoteApiDbContext> _authorRepository;
    private IRepository<AuthorAudit,QuoteApiDbContext> _authorAuditLogRepository;
    public event Action? NewQuoteAdded;
    public event Action<Quote>? GetLatestQuote;
    public QuoteService(IRepository<Quote,QuoteApiDbContext> quoteRepository, IRepository<Author,QuoteApiDbContext> authorRepository,NewQuoteNotifier notifier, IRepository<AuthorAudit,QuoteApiDbContext> authorAuditLogRepository)
    {
        _authorAuditLogRepository = authorAuditLogRepository;
        _quoteRepository = quoteRepository;
        _authorRepository = authorRepository;

        notifier.Subsribe(this);
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
            NewQuoteAdded?.Invoke();
            Quote? quote = await _quoteRepository.FindByAsync(expression);
            GetLatestQuote?.Invoke(quote);
            return quote;
        }
        return null;
    }
    public async Task<Quote?> GetQuoteById(int id)
    {
        Expression<Func<Quote,bool>> expression = quote => quote.Id==id;
        return await _quoteRepository.FindByAsync(expression);
    }
    public async Task<Author?> GetAuthorById(int id)
    {
        Expression<Func<Author,bool>> expression = author => author.Id==id;
        List<Author> authors = await _authorRepository.GetAllWithIncludeAsync(author => author.Quotes);
        Author? auth = null;
        foreach(Author author in authors)
        {
            if(author.Id == id) auth = author;
        }
        return auth;
    }

    public async Task<List<Quote>> GetQuotesWithAuthor()
    {
        Expression<Func<Quote,Author>> expression = quote => quote.Author;
        return await _quoteRepository.GetAllWithIncludeAsync(expression);
    }

    public async Task ModifyAuthorIdTracked(int authorId, int newId)
    {
        Expression<Func<Quote,bool>> expression = quote => quote.AuthorId==authorId;
        List<Quote> list = await _quoteRepository.GetTUsingWhereTracked(expression);
        foreach(Quote quote in list)
        {
            quote.AuthorId = newId;
        }
        await _quoteRepository.SaveChanges();
    }

    public async Task ModifyAuthorIdUntracked(int authorId, int newId)
    {
        Expression<Func<Quote,bool>> expression = quote => quote.AuthorId==authorId;
        List<Quote> list = await _quoteRepository.GetTUsingWhereUntracked(expression);
        foreach(Quote quote in list)
        {
            quote.AuthorId = newId;
        }
        await _quoteRepository.SaveChanges();
    }

    public async Task ModifyAuthorName(int id,string name)
    {

        Expression<Func<Author,bool>> expression = author => author.Id==id;
        Author? author = await _authorRepository.FindByAsync(expression);
        author.Name = name;
        await _authorRepository.SaveChanges();
    }

    public async Task<Author?> AddAuthor(string name)
    {
        Expression<Func<Author,bool>> expression = (author)=>author.Name==name;
        Author? author = await _authorRepository.FindByAsync(expression);
        if(author == null)
        {
            await using (IDbContextTransaction transaction = await _authorAuditLogRepository.BeginTransactionAsync())
            {
                try
                {
                    await _authorRepository.AddAsync(new Author(name){Name = name});
                    author = await _authorRepository.FindByAsync(expression);
                    if(author.Name.ToLower() == "muzan"){
                        await _authorAuditLogRepository.AddAsync(new AuthorAudit("Fail",author.Id,DateTime.UtcNow));
                        throw new Exception();
                        }
                    AuthorAudit audit = new AuthorAudit("Add",author.Id,DateTime.UtcNow);
                    await _authorAuditLogRepository.AddAsync(audit);
                    await transaction.CommitAsync();
                    return author;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        return null;
    }

}