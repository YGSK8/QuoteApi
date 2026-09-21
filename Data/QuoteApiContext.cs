namespace QuoteApi.Data;
using Microsoft.EntityFrameworkCore;
using QuoteApi.Models;
public class QuoteApiDbContext : DbContext
{
    public DbSet<Quote> Quotes {get;set;}
    public DbSet<Author> Authors {get;set;}
    public QuoteApiDbContext(DbContextOptions<QuoteApiDbContext> dbContextOptions):base(dbContextOptions)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().HasData(new Author("unknownAuthor"){Name = "unknownAuthor",Id = -1});
        modelBuilder.Entity<Quote>().Property(a=>a.AuthorId).HasDefaultValue(-1);
    }

}