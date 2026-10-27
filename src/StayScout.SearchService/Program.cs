using Microsoft.EntityFrameworkCore;
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

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<SearchGrpcService>();

app.Run();
