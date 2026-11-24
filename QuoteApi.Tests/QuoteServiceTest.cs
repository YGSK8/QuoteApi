using QuoteApi.Repositories;
using QuoteApi.Models;
using QuoteApi.Data;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using QuoteApi.Settings;
using QuoteApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace QuoteApi.Tests;

public class QuoteServiceTest : QuoteService,IQuoteService
{
    public QuoteApiDbContextTest _quoteApiDBContextTest;
    public QuoteServiceTest(IRepository<Quote,QuoteApiDbContext> quoteRepository, IRepository<Author,QuoteApiDbContext> authorRepository,NewQuoteNotifier notifier, IRepository<AuthorAudit,QuoteApiDbContext> authorAuditLogRepository, IOptions<AuthorSettings> authorSettings, QuoteApiDbContextTest quoteApiDbContextTest): base(quoteRepository,authorRepository,notifier,authorAuditLogRepository,authorSettings)
    {
        _quoteApiDBContextTest = quoteApiDbContextTest;
    }
    public override async Task ModifyAuthorName(int id,string name)
    {
        const string testname = "irrelevant";
        Expression<Func<Author,bool>> expression = author => author.Id==id;
        Author? trackedAuthor1 = await _authorRepository.FindByAsync(expression);
        trackedAuthor1.Name = name;
        Author? trackedAuthor2= await _quoteApiDBContextTest.Authors.FirstOrDefaultAsync(author=>author.Id==id);//pulling the same author with a second DB context
        trackedAuthor2.Name = testname; //changing the authorname via the second context
        await _quoteApiDBContextTest.SaveChangesAsync(); //saving the entity through the second context first, xmin version changes here...
        await _authorRepository.SaveChanges();//--attempting a save here through original DBContext should throw the DbUpdateConcurrencyException
    }
}