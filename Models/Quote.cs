namespace QuoteApi.Models;
// public record Quote(int Id, string Text);

public class Quote
{
    public int Id {get; set;}=0;
    public string Text{get; set;}
    public Author Author{get;set;}
    public int AuthorId{get;set;}
    public Quote(string text,int authorId)
    {
        Text = text;
        AuthorId = authorId;
    }
}
public record ClientQuote(string Text, string Author);
public record ClientQuoteResponse
{
    public int Id {get;}
    public string Text {get;}
    public ClientQuoteResponse(Quote quote)
    {
        Id = quote.Id;
        Text = quote.Text;
    }   
};