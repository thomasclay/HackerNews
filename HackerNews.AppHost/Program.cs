using System.Diagnostics.CodeAnalysis;

using HackerNewsModels;

namespace HackerNews.AppHost;

[ExcludeFromCodeCoverage()]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        // some issues with garnet and Lua support
        //var cache = builder.AddGarnet(Names.Cache)
        //    .WithArgs("--lua", "--lua-transaction-mode");
        var apiService = builder.AddProject<Projects.HackerNews_ApiService>(Names.ApiService)
            //.WithReference(cache)
            //.WaitFor(cache)
            .WithHttpHealthCheck("/health");

        builder.AddProject<Projects.HackerNews_Web>(Names.BlazorFrontEnd)
            .WithExternalHttpEndpoints()
            .WithHttpHealthCheck("/health")
            .WithReference(apiService)
            .WaitFor(apiService);

        builder.AddNpmApp(Names.AngularFrontEnd, "../AngularHackerNews")
            .WithReference(apiService)
            .WaitFor(apiService)
            .WithHttpEndpoint(env: "PORT")
            .WithExternalHttpEndpoints();

        builder.Build().Run();
    }
}
