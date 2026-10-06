namespace QuoteApi.Models;

public class AuthorAudit
{
    public int Id {get;set;}
    public string AuditType{get;set;}
    public Author Author{get;set;}
    public int AuthorId{get;set;}
    public DateTime DateTime{get;set;}

    public AuthorAudit(string auditType,int authorId,DateTime dateTime)
    {
        AuditType = auditType;
        AuthorId = authorId;
        DateTime = dateTime;
    }
}