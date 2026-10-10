using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using QuoteApi.Settings;
namespace QuoteApi.Models;

public class ClientAuthor
{
    [Required(ErrorMessage = "Empty strings are not allowed. Name cannot be whitespace only")]
    // [Required(AllowEmptyStrings = false, ErrorMessage = "Empty strings are not allowed. Name cannot be whitespace only")]
    public required string Name {get;set;}
}