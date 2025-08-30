using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

using Quartz;
using Refit;
using Scalar.AspNetCore;

using Common.Utility;
using HackerNewsModels;

using HackerNews.ApiService.Jobs;
using HackerNews.ApiService.Services;

namespace HackerNews.ApiService;

/// <summary>
/// Main program
/// </summary>
[ExcludeFromCodeCoverage()]
public static class Program
{
    /// <summary>
    /// Program entry point
    /// </summary>
    /// <param name="args">command line arguments</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults();
        builder.Services.AddSingleton<FactoryService>();

        builder.Services.AddDistributedMemoryCache();
        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            AllowOutOfOrderMetadataProperties = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = true,
        };
        builder.Services.AddSingleton(jsonSerializerOptions);
        builder.Services.AddSingleton<MessageCacheService>();

        builder.Services.AddScoped<IHackerNewsService, HackerNewsService>();
        builder.Services.AddOptions<MessageCacheServiceOptions>()
            .BindConfiguration("Cache");

        // Add services to the container.
        builder.Services.AddProblemDetails();

        // Quartz!
        builder.Services.AddQuartz(q =>
        {
            var delay = builder.Configuration.GetValue("JobSchedules:Updates", 1);

            q.AddJob<UpdatedItemsJob>(opts =>
            {
                opts.WithIdentity(UpdatedItemsJob.Key)
                    .DisallowConcurrentExecution();
            });
            q.AddTrigger(opts =>
            {
                opts.ForJob(UpdatedItemsJob.Key)
                    .WithIdentity(UpdatedItemsJob.Key.ToString())
                    .StartNow()
                    .WithSimpleSchedule(sb => sb.WithIntervalInMinutes(delay));
            });

            delay = builder.Configuration.GetValue("JobSchedules:Lists", 1);

            q.AddJob<GetListsJob>(opts =>
            {
                opts.WithIdentity(GetListsJob.Key)
                    .DisallowConcurrentExecution();
            });
            q.AddTrigger(opts =>
            {
                opts.ForJob(GetListsJob.Key)
                    .WithIdentity(GetListsJob.Key.ToString())
                    .StartNow()
                    .WithSimpleSchedule(sb => sb.WithIntervalInMinutes(delay));
            });
        });

        builder.Services.AddQuartzHostedService(q =>
        {
            q.WaitForJobsToComplete = true;
            q.AwaitApplicationStarted = true;
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var refitSettings = new RefitSettings()
        {
            ContentSerializer = new SystemTextJsonContentSerializer(jsonSerializerOptions),            
        };
        builder.Services.AddRefitClient<IHackerNewsApi>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new("https://hacker-news.firebaseio.com"));

        builder.Services.AddControllers()
            .AddJsonOptions(config =>
            {
                config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                config.JsonSerializerOptions.AllowOutOfOrderMetadataProperties = true;
                config.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

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
}
