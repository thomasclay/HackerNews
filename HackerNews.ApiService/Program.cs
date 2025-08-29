using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using Refit;
using Scalar.AspNetCore;

using HackerNewsModels;

using HackerNews.ApiService.Services;

namespace HackerNews.ApiService;

[ExcludeFromCodeCoverage()]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults();
        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSingleton<MessageCacheService>();
        builder.Services.AddScoped<IHackerNewsService, HackerNewsService>();
        builder.Services.Configure<MessageCacheServiceOptions>(cacheOptions =>
        {
            if (int.TryParse(builder.Configuration[Names.CacheEntryLifeInHours], out var hours))
            {
                cacheOptions.CacheExpirationHours = hours;
            }
        });
        
        // Add services to the container.
        builder.Services.AddProblemDetails();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddRefitClient<Services.IHackerNewsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new("https://hacker-news.firebaseio.com"));

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

        app.MapGet("/weatherforecast", () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast");
        
        app.MapControllers();
        
        app.MapDefaultEndpoints();

        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
