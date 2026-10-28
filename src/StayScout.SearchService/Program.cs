using Microsoft.EntityFrameworkCore;
using StayScout.SearchService;
using StayScout.SearchService.Abstractions;
using StayScout.SearchService.Application;
using StayScout.SearchService.Grpc;
using StayScout.SearchService.Infrastructure.Persistence;
using StayScout.SearchService.Providers;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("SearchDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'SearchDatabase' is not configured.");

builder.Services.AddDbContext<SearchDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ISearchDbAccess, SearchDbAccess>();
builder.Services.AddScoped<ISearchApplicationService, SearchApplicationService>();

builder.Services.AddSingleton<IHotelProvider, InMemoryHotelProvider>();

builder.Services.AddSearchMessaging(builder.Configuration);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<SearchGrpcService>();

app.Run();
