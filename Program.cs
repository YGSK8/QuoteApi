using QuoteApi.Models;
using QuoteApi.Repositories;
using QuoteApi.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<IQuoteService,QuoteService>();
builder.Services.AddSingleton<IRepository<Quote>,Repository<Quote>>();
WebApplication app = builder.Build();
IRepository<Quote> repository = app.Services.GetRequiredService<IRepository<Quote>>();
repository.Add(new(1,"testing"));
app.MapControllers();
app.Run();