using MarketBook.Application;
using MarketBook.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// Services
// -----------------------------------------------------------------------------

builder.Services.AddControllers();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    string xmlFile =
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";

    string xmlPath = Path.Combine(
        AppContext.BaseDirectory,
        xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

// -----------------------------------------------------------------------------
// Application
// -----------------------------------------------------------------------------

WebApplication app = builder.Build();

// -----------------------------------------------------------------------------
// HTTP Request Pipeline
// -----------------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "MarketBook API v1");

        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
