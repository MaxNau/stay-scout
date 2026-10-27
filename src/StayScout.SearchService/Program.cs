using Microsoft.EntityFrameworkCore;
using StayScout.SearchService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration
    .GetConnectionString("SearchDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'SearchDatabase' is not configured.");

builder.Services.AddDbContext<SearchDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddGrpc();

var app = builder.Build();

app.Run();
