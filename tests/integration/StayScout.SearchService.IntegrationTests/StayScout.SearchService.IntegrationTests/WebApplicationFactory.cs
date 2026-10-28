using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using StayScout.SearchService.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using TUnit.Core.Interfaces;

namespace StayScout.SearchService.IntegrationTests;

public sealed class SearchServiceWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncInitializer
{
    private readonly PostgreSqlContainer postgres =
        new PostgreSqlBuilder("postgres:17-alpine")
            .Build();

    private readonly RabbitMqContainer rabbitMq =
        new RabbitMqBuilder("rabbitmq:4-alpine")
            .Build();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            postgres.StartAsync(),
            rabbitMq.StartAsync());

        var options = new DbContextOptionsBuilder<SearchDbContext>()
            .UseNpgsql(postgres.GetConnectionString())
            .Options;

        await using (var dbContext = new SearchDbContext(options))
        {
            await dbContext.Database.MigrateAsync();
        }

        Environment.SetEnvironmentVariable(
            "ConnectionStrings__SearchDatabase",
            postgres.GetConnectionString());

        Environment.SetEnvironmentVariable(
            "RabbitMq__Uri",
            rabbitMq.GetConnectionString());

        _ = Server;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await postgres.DisposeAsync();
        await rabbitMq.DisposeAsync();
    }
}
