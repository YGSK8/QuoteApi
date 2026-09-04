using QuoteApi.Models;
using QuoteApi.Repositories;
using QuoteApi.Services;
using QuoteApi.Middlewares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<IQuoteService,QuoteService>();
builder.Services.AddSingleton<IRepository<Quote>,Repository<Quote>>();
builder.Services.AddSingleton<NewQuoteNotifier>();
WebApplication app = builder.Build();
IRepository<Quote> repository = app.Services.GetRequiredService<IRepository<Quote>>();
app.Services.GetRequiredService<NewQuoteNotifier>();
repository.Add(new(1,"testing"));
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();
app.Run();
