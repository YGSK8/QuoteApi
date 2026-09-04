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
    public IActionResult GetAll()
    {
        return Ok(_quoteService.GetQuotes());
    }

    [HttpPost]
    public IActionResult AddQuote([FromBody] ClientQuote clientquote)
    {
        if(String.IsNullOrWhiteSpace(clientquote.Text)) return BadRequest("Text cannot be empty or consist only of white-space characters");
        Quote? quote = _quoteService.AddQuote(clientquote.Text);
        if(quote==null)return BadRequest($"Quote already exists");
        return Ok(quote);
    }

    [Route("{id}")]
    [HttpGet]
    public IActionResult GetQuoteById(int id)
    {
        Quote? quote = _quoteService.GetQuoteById(id);
        if(quote==null)return NotFound($"Quote with id {id} does not exist");
        else return Ok(quote);
    }

    [Route("MiddlewareExceptionTest")]
    [HttpGet]
    
    public IActionResult ThrowException()
    {
        throw new Exception("This might contain sensitive information and should not be exposed to the client!");
    }

}