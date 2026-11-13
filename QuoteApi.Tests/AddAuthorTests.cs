using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuoteApi.Settings;
using QuoteApi.Data;
using QuoteApi.Services;
using Microsoft.EntityFrameworkCore;

namespace QuoteApi.Tests;
public class AddAuthorTests:IClassFixture<QuoteApiFactory>
{
    private readonly HttpClient _client;
    private readonly  QuoteApiFactory _factory;
    public AddAuthorTests( QuoteApiFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    [Fact]
    public void App_UsesTestDatabase()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        QuoteApiDbContext context = scope.ServiceProvider.GetRequiredService<QuoteApiDbContext>();
        string connectionString = context.Database.GetDbConnection().ConnectionString;
        Assert.Equal("quoteapi_test", connectionString.Split(';').Select(p => p.Split('=')).FirstOrDefault(p => p[0].Equals("Database", StringComparison.OrdinalIgnoreCase))?[1]);
    }
    [Fact]
    public async Task AddAuthor_EmptyName_Returns400()
    {
        JsonContent body = JsonContent.Create(new {name = ""});
        HttpResponseMessage response = await _client.PostAsync("/quotes/AddAuthor",body);
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
    }
     [Fact]
    public async Task AddAuthor_WhiteSpaceName_Returns400()
    {
        JsonContent body = JsonContent.Create(new {name = "    "});
        HttpResponseMessage response = await _client.PostAsync("/quotes/AddAuthor",body);
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
    }
    [Fact]
    public async Task AddAuthor_NoNameProperty_Returns400()
    {
        JsonContent body = JsonContent.Create(new{});
        HttpResponseMessage response = await _client.PostAsync("/quotes/AddAuthor",body);
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
    }
    [Fact]
    public async Task AddAuthor_NameTooLong_Returns400()
    {
        int max = _factory.Services.GetRequiredService<IOptions<AuthorSettings>>().Value.MaxNameLength;
        JsonContent body = JsonContent.Create(new {name = new string('a',max+1)});//--sends a name with length +1 above limit. Expecting Bad request with error message.
        HttpResponseMessage responseMessage = await _client.PostAsync("/quotes/AddAuthor",body);
        string response = await responseMessage.Content.ReadAsStringAsync();
        Assert.Contains("Text length exceeds allowable limit",response);//--checks if error message is sent
        Assert.Equal(HttpStatusCode.BadRequest,responseMessage.StatusCode);//-- then check whether the correct statuscode is thrown
    }

}
