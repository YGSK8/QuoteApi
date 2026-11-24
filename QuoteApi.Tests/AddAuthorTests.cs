using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuoteApi.Settings;
using QuoteApi.Data;
using QuoteApi.Services;
using Microsoft.EntityFrameworkCore;
using QuoteApi.Models;
using QuoteApi.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using Microsoft.AspNetCore.Mvc.Testing;

namespace QuoteApi.Tests;
public class AddAuthorTests:IClassFixture<QuoteApiFactory>,IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly  QuoteApiFactory _factory;
    private readonly string _testAuthorName ="Marco";
    public async Task InitializeAsync()
    {
        await CleanUpTestAuthorRows(_testAuthorName);
    }
    public async Task DisposeAsync()
    {
        await CleanUpTestAuthorRows(_testAuthorName);
    }
    private async Task CleanUpTestAuthorRows(string authorName)
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        QuoteApiDbContext context = scope.ServiceProvider.GetRequiredService<QuoteApiDbContext>();
        await context.Authors.Where<Author>(author=>author.Name==authorName).ExecuteDeleteAsync();
    }
    public AddAuthorTests(QuoteApiFactory factory) 
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
    [Fact]
    public async Task AddAuthor_ToTestDatabase_Returns200()
    {
        ClientAuthor author = new ClientAuthor {Name = _testAuthorName};
        JsonContent body = JsonContent.Create(author);
        HttpResponseMessage responseMessage = await _client.PostAsync("/quotes/AddAuthor",body);
        string responseBody = await responseMessage.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK,responseMessage.StatusCode);
        int id = int.Parse(responseBody.Split("with Id ")[1]);
        using IServiceScope scope = _factory.Services.CreateScope();
        IQuoteService service = scope.ServiceProvider.GetRequiredService<IQuoteService>();
        Author? author1 = await service.GetAuthorById(id);
        Assert.Equal(_testAuthorName,author1?.Name);
        Assert.Equal(id,author1?.Id);
    }
    [Fact]
    public async Task AddAuthor_Duplicate_Returns409()
    {
        ClientAuthor author = new ClientAuthor{Name = _testAuthorName};
        JsonContent body = JsonContent.Create(author);
        HttpResponseMessage firstResponseMessage = await _client.PostAsync("/quotes/AddAuthor",body);
        Assert.Equal(HttpStatusCode.OK,firstResponseMessage.StatusCode);
        HttpResponseMessage secondResponseMessage = await _client.PostAsync("/quotes/AddAuthor",body);
        Assert.Equal(HttpStatusCode.Conflict,secondResponseMessage.StatusCode);
    }
    [Fact]
    public async Task AddAuthor_MaxLength_Returns200()
    {
        int maxLength = _factory.Services.GetRequiredService<IOptions<AuthorSettings>>().Value.MaxNameLength;
        string name = new string('a',maxLength);
        await CleanUpTestAuthorRows(name);
        ClientAuthor author = new ClientAuthor{Name=name};
        JsonContent body = JsonContent.Create(author);
        try
        {
            HttpResponseMessage responseMessage = await _client.PostAsync("/quotes/AddAuthor",body);
            string text = await responseMessage.Content.ReadAsStringAsync();
            string extractedName = text.Split(" has been added with Id ")[0];
            Assert.Equal(HttpStatusCode.OK,responseMessage.StatusCode);
            Assert.Equal(maxLength,extractedName.Length);
        }
        finally
        {
             await CleanUpTestAuthorRows(name);
        }
    }

    [Fact]
    public async Task RenameAuthorById_Success_Returns200()
    {
        const string name = "Polo";
        try
        {
            ClientAuthor clientAuthor = new ClientAuthor{Name = _testAuthorName};
            JsonContent jsonContent = JsonContent.Create(clientAuthor);
            HttpResponseMessage addAuthorResponseMessage = await _client.PostAsync("/quotes/AddAuthor",jsonContent);
            int? id;
            using(IServiceScope scope = _factory.Services.CreateScope())
            {
                QuoteApiDbContext dbContext = scope.ServiceProvider.GetRequiredService<QuoteApiDbContext>();
                Author? testAuthor = await dbContext.Authors.FirstOrDefaultAsync<Author>(author=> author.Name==_testAuthorName);
                id = testAuthor?.Id;
                Assert.Equal(HttpStatusCode.OK,addAuthorResponseMessage.StatusCode);//--successfully uploaded _testAuthorName
                Assert.Equal(_testAuthorName,testAuthor?.Name);//--successfully uploaded _testAuthorName
            }
            using(IServiceScope scope = _factory.Services.CreateScope())
            {
                QuoteApiDbContext dbContext = scope.ServiceProvider.GetRequiredService<QuoteApiDbContext>();
                HttpResponseMessage modifyAuthorResponseMessage = await _client.PostAsync($"/quotes/Author/modify?id={id}&name={name}",null);
                Author? modifiedAuthor = await dbContext.Authors.FirstOrDefaultAsync(author => author.Id==id);
                Assert.Equal(HttpStatusCode.OK,modifyAuthorResponseMessage.StatusCode);
                Assert.Equal(name,modifiedAuthor?.Name);
            }
        }
        finally
        {
            await CleanUpTestAuthorRows(name);
        }
    }
    [Fact]
    public async Task RenameAuthorById_FailDueToSameName_Returns400()
    {
        ClientAuthor author = new ClientAuthor{Name=_testAuthorName};
        JsonContent jsonContent = JsonContent.Create(author);
        HttpResponseMessage addTestAuthorMessage = await _client.PostAsync("/quotes/AddAuthor",jsonContent);
        Assert.Equal(HttpStatusCode.OK,addTestAuthorMessage.StatusCode);
        using IServiceScope scope = _factory.Services.CreateScope();
        QuoteApiDbContext dbContext = scope.ServiceProvider.GetRequiredService<QuoteApiDbContext>();
        Author? testAuthor = await dbContext.Authors.FirstOrDefaultAsync(author=>author.Name==_testAuthorName);
        Assert.NotNull(testAuthor); //--ensures testAuthor is not null because if author is null, so is id, and url validation will fail with status 400 as well
        int id = testAuthor.Id;
        Assert.Equal(_testAuthorName,testAuthor.Name);
        HttpResponseMessage modifyAuthorWithSameNameMessage = await _client.PostAsync($"/quotes/Author/modify?id={id}&name={_testAuthorName}",null);
        Assert.Equal(HttpStatusCode.BadRequest,modifyAuthorWithSameNameMessage.StatusCode);
    }
    [Fact]
    public async Task RenameAuthorById_FailDueToInvalidId_Returns404()
    {
        const string name = "irrelevant";
        const int incorrectId = -99999999;
        HttpResponseMessage modifyAuthorWithIncorrectId = await _client.PostAsync($"/quotes/Author/modify?id={incorrectId}&name={name}",null);
        Assert.Equal(HttpStatusCode.NotFound,modifyAuthorWithIncorrectId.StatusCode);
    }

    [Fact]
    public async Task RenameAuthorById_FailDueToConcurrencyException_Returns409()
    {
        await using (WebApplicationFactory<Program> secondFactory = _factory.WithWebHostBuilder((builder) =>
        {
            builder.ConfigureServices((serviceCollection) =>
            {
                List<ServiceDescriptor> serviceDescriptors = serviceCollection.ToList();
                ServiceDescriptor quoteService = null;
                foreach(ServiceDescriptor serviceDescriptor in serviceDescriptors)
                {
                    if(serviceDescriptor.ServiceType == typeof(IQuoteService)) quoteService = serviceDescriptor;
                }
                serviceCollection.Remove(quoteService);
                serviceCollection.AddScoped<IQuoteService,QuoteServiceTest>();
                string connectionString = builder.GetSetting("ConnectionStrings:QuoteApi");
                serviceCollection.AddDbContext<QuoteApiDbContextTest>((options)=>options.UseNpgsql(connectionString));
            });
        }))
        {
            const string newName = "modified";
            using IServiceScope scope = secondFactory.Services.CreateScope();
            QuoteServiceTest serviceTest = (QuoteServiceTest)scope.ServiceProvider.GetRequiredService<IQuoteService>();
            QuoteApiDbContext dbContext = scope.ServiceProvider.GetRequiredService<QuoteApiDbContext>();
            string connexString = serviceTest._quoteApiDBContextTest.Database.GetDbConnection().ConnectionString;
            Assert.Equal("quoteapi_test", connexString.Split(';').Select(p => p.Split('=')).FirstOrDefault(p => p[0].Equals("Database", StringComparison.OrdinalIgnoreCase))?[1]);//--just making sure second factory is using the correct connection string
            HttpClient client = secondFactory.CreateClient();
            ClientAuthor author = new ClientAuthor{Name = _testAuthorName};
            JsonContent jsonContent = JsonContent.Create(author);
            HttpResponseMessage httpResponseMessage = await client.PostAsync("/quotes/AddAuthor",jsonContent);
            string message = await httpResponseMessage.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK,httpResponseMessage.StatusCode);//--successfully added author
            Author trackedAuthor = dbContext.Authors.FirstOrDefault((author)=> author.Name == _testAuthorName);
            int id = trackedAuthor.Id;
            try
            {
                HttpResponseMessage httpResponseMessageModifyAuthor = await client.PostAsync($"/quotes/Author/modify?id={id}&name={newName}",null);
                string messageModifyAuthor = await httpResponseMessageModifyAuthor.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.Conflict,httpResponseMessageModifyAuthor.StatusCode);
                Assert.Contains("due to concurrency conflict",messageModifyAuthor); 
            }
            finally
            {
                await dbContext.Authors.Where((author)=>author.Id==id).ExecuteDeleteAsync(); //--cleaning up row added 
            }
        }
    }
}
