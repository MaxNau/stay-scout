using Grpc.Core;
using Grpc.Net.Client;
using StayScout.Contracts.Grpc;
using SearchGrpc = StayScout.Contracts.Grpc.SearchGrpcService;

namespace StayScout.SearchService.IntegrationTests;

public class SearchTests
{
    [ClassDataSource<SearchServiceWebApplicationFactory>(
        Shared = SharedType.PerTestSession)]
    public required SearchServiceWebApplicationFactory Factory { get; init; }

    [Test]
    public async Task StartSearch_ReturnsSearchId()
    {
        using var channel = CreateChannel();
        var client = new SearchGrpc.SearchGrpcServiceClient(channel);

        var response = await client.StartSearchAsync(
            new StartSearchRequest { Query = "hotels in Kyiv" });

        await Assert.That(Guid.TryParse(response.SearchId, out var searchId)).IsTrue();
        await Assert.That(searchId).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task GetSearch_AfterStartingSearch_ReturnsSearch()
    {
        using var channel = CreateChannel();
        var client = new SearchGrpc.SearchGrpcServiceClient(channel);

        var started = await client.StartSearchAsync(
            new StartSearchRequest { Query = "hotels in Kyiv" });

        var result = await client.GetSearchAsync(
            new GetSearchRequest { SearchId = started.SearchId });

        await Assert.That(result.SearchId).IsEqualTo(started.SearchId);
    }

    [Test]
    public async Task StartSearch_EmptyQuery_ReturnsInvalidArgument()
    {
        using var channel = CreateChannel();
        var client = new SearchGrpc.SearchGrpcServiceClient(channel);

        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await client.StartSearchAsync(
                new StartSearchRequest { Query = "" }));

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception.StatusCode)
            .IsEqualTo(StatusCode.InvalidArgument);
    }

    [Test]
    public async Task GetSearch_InvalidSearchId_ReturnsInvalidArgument()
    {
        using var channel = CreateChannel();
        var client = new SearchGrpc.SearchGrpcServiceClient(channel);

        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await client.GetSearchAsync(
                new GetSearchRequest { SearchId = "not-a-guid" }));

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception.StatusCode)
            .IsEqualTo(StatusCode.InvalidArgument);
    }

    [Test]
    public async Task GetSearch_NonexistentSearch_ReturnsNotFound()
    {
        using var channel = CreateChannel();
        var client = new SearchGrpc.SearchGrpcServiceClient(channel);

        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await client.GetSearchAsync(
                new GetSearchRequest { SearchId = Guid.NewGuid().ToString() }));

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception.StatusCode)
            .IsEqualTo(StatusCode.NotFound);
    }

    private GrpcChannel CreateChannel() =>
        GrpcChannel.ForAddress(
            "http://localhost",
            new GrpcChannelOptions
            {
                HttpHandler = Factory.Server.CreateHandler()
            });
}
