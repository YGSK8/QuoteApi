namespace QuoteApi.Data;
using Microsoft.EntityFrameworkCore;
using QuoteApi.Models;
public class QuoteApiDbContext : DbContext
{
    public DbSet<Quote> Quotes {get;set;}

    public QuoteApiDbContext(DbContextOptions<QuoteApiDbContext> dbContextOptions):base(dbContextOptions)
    {
        
    }

}