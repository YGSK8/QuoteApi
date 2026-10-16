using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuoteApi.Settings;
namespace QuoteApi.Tests;
public class AddAuthorTests:IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    public AddAuthorTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
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
