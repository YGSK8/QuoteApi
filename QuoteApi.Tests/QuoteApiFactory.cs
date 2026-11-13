using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

namespace QuoteApi.Tests;

public class QuoteApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:QuoteApi", "Host=localhost;Port=5432;Database=quoteapi_test;Username=yanishgooradoo");
    }
}