using Microsoft.AspNetCore.Mvc.Testing;
using TUnit.Core.Interfaces;

namespace StayScout.SearchService.IntegrationTests;

public class SearchServiceWebApplicationFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
    public Task InitializeAsync()
    {
        _ = Server;

        return Task.CompletedTask;
    }
}
