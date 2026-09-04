namespace QuoteApi.Middlewares;
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            Console.WriteLine("getting in the try block...");
            await _next(context);
            Console.WriteLine("getting in the try block again...");
        }
        catch (Exception ex)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 500;
                Console.WriteLine(ex);
                await context.Response.WriteAsync($"An unexpected error occurred");
            }
        }
    }
}