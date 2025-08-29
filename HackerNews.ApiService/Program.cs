using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using Refit;
using Scalar.AspNetCore;

using HackerNewsModels;

using HackerNews.ApiService.Services;
using Quartz;
using HackerNews.ApiService.Jobs;

namespace HackerNews.ApiService;

[ExcludeFromCodeCoverage()]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults();

        builder.Services.AddMemoryCache();

        builder.Services.AddSingleton<MessageCacheService>();
        builder.Services.AddSingleton<LoadedStories>();

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

        // Quartz!
        builder.Services.AddQuartz(q =>
        {
            AddStoriesJobs(q, builder.Configuration.GetSection("JobSchedules"));
        });

        builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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

        app.MapControllers();

        app.MapDefaultEndpoints();

        app.Run();
    }

    static void AddStoriesJobs(IServiceCollectionQuartzConfigurator q, IConfigurationSection configuration)
    {
        foreach (var category in Enum.GetValues<IHackerNewsService.StoryCategory>())
        {
            var delay = configuration.GetValue(category.ToString(), 5);

            var dataMap = new JobDataMap()
            {
                { nameof(category), category }
            };

            var jobKey = new JobKey(category.ToString(), "GetStories");
            q.AddJob<GetStories>(opts =>
            {
                opts.WithIdentity(jobKey)
                    .SetJobData(dataMap)
                    .DisallowConcurrentExecution();
            });
            q.AddTrigger(opts =>
            {
                opts.ForJob(jobKey)
                    .WithIdentity(jobKey.ToString())
                    .StartNow()
                    .WithSimpleSchedule(sb => sb.WithIntervalInMinutes(delay));
            });
        }
    }
}
