using MarketBook.Application;
using MarketBook.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// Services
// -----------------------------------------------------------------------------

builder.Services.AddControllers();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

// -----------------------------------------------------------------------------
// Application
// -----------------------------------------------------------------------------

WebApplication app = builder.Build();

// -----------------------------------------------------------------------------
// HTTP Request Pipeline
// -----------------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
