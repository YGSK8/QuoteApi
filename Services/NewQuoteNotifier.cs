using QuoteApi.Models;

namespace QuoteApi.Services;
public class NewQuoteNotifier
{
    private void Notifier()
    {
        Console.WriteLine("Simulating a notification - We have a new quote, please open your inbox to check!");
    }
    private void UploadQuote(Quote quote)
    {
        Console.WriteLine($"Simulating uploading the Quote:'{quote.Text}' to an inbox");
    }
    public void Subsribe(IQuoteService service)
    {
        service.NewQuoteAdded+=Notifier;
        service.GetLatestQuote+=UploadQuote;
    }
}