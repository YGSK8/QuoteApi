using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
namespace QuoteApi.Tests;
public class AddAuthorTests
{
    [Fact]
    public async Task AddAuthor_EmptyName_Returns400()
    {
        WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();
        HttpClient client = factory.CreateClient();
        JsonContent body = JsonContent.Create(new {name = ""});
        HttpResponseMessage response = await client.PostAsync("/quotes/AddAuthor",body);
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
    }

}
