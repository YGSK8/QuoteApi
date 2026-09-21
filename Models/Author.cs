namespace QuoteApi.Models;

public class Author
{
    public int Id {get;set;}=0;
    public required string Name {get; set;}
    public List<Quote> Quotes {get;set;} = new List<Quote>();

    public Author (string name)
    {
        Name = name;
    }
}