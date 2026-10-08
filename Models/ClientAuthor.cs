using System.ComponentModel.DataAnnotations;
namespace QuoteApi.Models;

public class ClientAuthor
{
    [StringLength(30, ErrorMessage = "Name length can't be more than 30.")]
    [Required(ErrorMessage = "Empty strings are not allowed. Name cannot be whitespace only")]
    // [Required(AllowEmptyStrings = false, ErrorMessage = "Empty strings are not allowed. Name cannot be whitespace only")]
    public required string Name {get;set;}
}