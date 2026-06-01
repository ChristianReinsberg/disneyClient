using System.Text.Json.Serialization;
using DisneyApi.Data;
using DisneyApi.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
            providerOptions => providerOptions.EnableRetryOnFailure())
           .UseSnakeCaseNamingConvention());
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:8080")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient("tmdbClient", client =>
{   
    var api_key = builder.Configuration["TmdbApiKey"];
    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
    client.DefaultRequestHeaders.Add("accept", "application/json");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {api_key}");
});
builder.Services.AddHttpClient("disneyClient", client =>
{
    client.BaseAddress = new Uri("https://api.disneyapi.dev/");
    client.DefaultRequestHeaders.Add("User-Agent", "DisneyPortfolioApp/1.0");
});

builder.Services.AddScoped<TmdbService>();
builder.Services.AddScoped<DisneyService>();
builder.Services.AddHostedService<DisneyCrawlerService>();

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowReactApp");

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<Logger<Program>>();
        logger.LogError(ex, "error at automatic database migration");
    }
}

app.Run();

public partial class Program{}