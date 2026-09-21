using Microsoft.AspNetCore.Mvc;
using QuoteApi.Repositories;
using QuoteApi.Services;
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

    [HttpPost]
    public async Task<IActionResult> AddQuote([FromBody] ClientQuote clientquote)
    {
        if(String.IsNullOrWhiteSpace(clientquote.Text)||String.IsNullOrWhiteSpace(clientquote.Author)) return BadRequest("Text cannot be empty or consist only of white-space characters");
        Quote? quote = await _quoteService.AddQuoteAsync(clientquote.Text,clientquote.Author);
        if(quote==null)return BadRequest($"Quote already exists");
        return Ok(new ClientQuoteResponse(quote));
    }

    [Route("{id}")]
    [HttpGet]
    public async Task<IActionResult> GetQuoteById(int id)
    {
        Quote? quote = await _quoteService.GetQuoteById(id);
        if(quote==null)return NotFound($"Quote with id {id} does not exist");
        else return Ok(new ClientQuoteResponse(quote));
    }

    [Route("MiddlewareExceptionTest")]
    [HttpGet]
    
    public IActionResult ThrowException()
    {
        throw new Exception("This might contain sensitive information and should not be exposed to the client!");
    }

}