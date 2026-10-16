using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
namespace QuoteApi.Tests;
public class AddAuthorTests:IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public AddAuthorTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
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

}
