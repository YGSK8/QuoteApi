using QuoteApi.Data;
using Microsoft.EntityFrameworkCore;
using QuoteApi.Models;
namespace QuoteApi.Tests;

public class QuoteApiDbContextTest : QuoteApiDbContext
{
    public QuoteApiDbContextTest(DbContextOptions<QuoteApiDbContext> dbContextOptions) : base(dbContextOptions)
    {
        
    }
}