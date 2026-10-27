using Microsoft.EntityFrameworkCore;
using StayScout.Contracts;
using StayScout.SearchService;
using StayScout.SearchService.Abstractions;
using StayScout.SearchService.Application;
using StayScout.SearchService.Grpc;
using StayScout.SearchService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration
    .GetConnectionString("SearchDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'SearchDatabase' is not configured.");

builder.Services.AddDbContext<SearchDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ISearchDbAccess, SearchDbAccess>();
builder.Services.AddScoped<ISearchApplicationService, SearchApplicationService>();

builder.Services.AddSearchMessaging(builder.Configuration);

builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5226, listenOptions =>
    {
        listenOptions.Protocols =
            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });
});

var app = builder.Build();

app.MapPost("/debug/publish", async (
    MassTransit.IPublishEndpoint publishEndpoint,
    CancellationToken cancellationToken) =>
{
    var searchId = Guid.NewGuid();

    await publishEndpoint.Publish(
        new SearchRequested(searchId),
        cancellationToken);

    return Results.Ok(new { SearchId = searchId });
});

app.MapGrpcService<SearchGrpcService>();

app.Run();
