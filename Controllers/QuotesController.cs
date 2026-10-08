using Microsoft.AspNetCore.Mvc;
using QuoteApi.Repositories;
using QuoteApi.Services;
using Microsoft.EntityFrameworkCore;
using QuoteApi.Models;

namespace QuoteApi.Controllers;
[ApiController]
[Route("quotes")]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;
    public QuotesController(IQuoteService service)
    {
        _quoteService = service;
    }
    
    [HttpGet]
    public IActionResult GetRandom()
    {
        string randomQuote = _quoteService.GenerateRandomQuote(); 
        return Ok(randomQuote);
    }
    [Route("all")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        List<ClientQuoteResponse> response = new List<ClientQuoteResponse>();
        foreach(Quote quote in await _quoteService.GetQuotes())
        {
            response.Add(new ClientQuoteResponse(quote));
        }
        return Ok(response);
    }
    [Route("all/author")]
    [HttpGet]
    public async Task<IActionResult> GetAllWithAuthor()
    {
        List<ClientQuoteResponse> response = new List<ClientQuoteResponse>();
        foreach(Quote quote in await _quoteService.GetQuotesWithAuthor())
        {
            response.Add(new ClientQuoteResponse(quote));
        }
        return Ok(response);
    }

    [Route("modifyT/{authorId}/{newId}")]
    [HttpPost]
    public async Task<IActionResult> ModifyQuoteTracked(int authorId,int newId)
    {
        await _quoteService.ModifyAuthorIdTracked(authorId,newId);
        List<ClientQuoteResponse> response = new List<ClientQuoteResponse>();
        foreach(Quote quote in await _quoteService.GetQuotes())
        {
            response.Add(new ClientQuoteResponse(quote));
        }
        return Ok(response);
    } 

    [Route("modifyU/{authorId}/{newId}")]
    [HttpPost]
    public async Task<IActionResult> ModifyQuoteUntracked(int authorId,int newId)
    {
        await _quoteService.ModifyAuthorIdUntracked(authorId,newId);
        List<ClientQuoteResponse> response = new List<ClientQuoteResponse>();
        foreach(Quote quote in await _quoteService.GetQuotes())
        {
            response.Add(new ClientQuoteResponse(quote));
        }
        return Ok(response);
    } 
    

    [HttpPost]
    public async Task<IActionResult> AddQuote([FromBody] ClientQuote clientquote)
    {
        if(String.IsNullOrWhiteSpace(clientquote.Text)||String.IsNullOrWhiteSpace(clientquote.Author)) return BadRequest("Text cannot be empty or consist only of white-space characters");
        Quote? quote = await _quoteService.AddQuoteAsync(clientquote.Text,clientquote.Author);
        if(quote==null)return BadRequest($"Quote already exists");
        return Ok(new ClientQuoteResponse(quote));
    }

    [Route("AddAuthor")]
    [HttpPost]
    public async Task<IActionResult> AddAuthor(ClientAuthor authorName)
    {
        Author? author = await _quoteService.AddAuthor(authorName.Name);
        if(author==null) return Conflict($"Author already exists");
        return Ok($"{author.Name} has been added with Id {author.Id}");
    }

    [Route("{id}")]
    [HttpGet]
    public async Task<IActionResult> GetQuoteById(int id)
    {
        Quote? quote = await _quoteService.GetQuoteById(id);
        if(quote==null)return NotFound($"Quote with id {id} does not exist");
        else return Ok(new ClientQuoteResponse(quote));
    }

    [Route("Author/{id}")]
    [HttpGet]
    public async Task<IActionResult> GetAuthorById(int id)
    {
        Author? author = await _quoteService.GetAuthorById(id);
        if(author == null) return NotFound($"Quote with id {id} does not exist");
        else {
            foreach(Quote quote in author.Quotes)
            {
                Console.WriteLine(quote.Text);
            }
            return Ok($"{author.Name} had {author.Quotes.Count} quotes");}
    }

    [Route("Author/modify")]
    [HttpPost]
    public async Task<IActionResult> RenameAuthorByid(int id, string name)
    {
        try
        {
            await _quoteService.ModifyAuthorName(id,name);
            return Ok("modification successful");
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict($"Unable to modify Author with id {id} due to concurrency conflict. Refresh and try again.");
        }
    }

    [Route("MiddlewareExceptionTest")]
    [HttpGet]
    
    public IActionResult ThrowException()
    {
        throw new Exception("This might contain sensitive information and should not be exposed to the client!");
    }

}