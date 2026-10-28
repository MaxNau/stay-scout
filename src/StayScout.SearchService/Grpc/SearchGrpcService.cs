using Grpc.Core;
using StayScout.Contracts.Grpc;
using StayScout.SearchService.Abstractions;
using SearchGrpc = StayScout.Contracts.Grpc.SearchGrpcService;

namespace StayScout.SearchService.Grpc;

public sealed class SearchGrpcService(
    ISearchApplicationService searchApplicationService)
    : SearchGrpc.SearchGrpcServiceBase
{
    public override async Task<StartSearchResponse> StartSearch(
        StartSearchRequest request,
        ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Query is required."));
        }

        var searchId = await searchApplicationService.StartSearchAsync(
            request.Query,
            context.CancellationToken);

        return new StartSearchResponse
        {
            SearchId = searchId.ToString()
        };
    }

    public override async Task<GetSearchResponse> GetSearch(
        GetSearchRequest request,
        ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.SearchId))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Search ID is required."));
        }

        if (!Guid.TryParse(request.SearchId, out var searchId))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Search ID must be a valid GUID."));
        }

        var search = await searchApplicationService.GetSearchAsync(
            searchId,
            context.CancellationToken);

        if (search is null)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, "Search was not found."));
        }

        var response = new GetSearchResponse
        {
            SearchId = search.Id.ToString(),
            IsCompleted = search.IsCompleted
        };

        response.Results.AddRange(search.Results.Select(result => new SearchResult
        {
            Id = result.Id,
            Name = result.Name,
            City = result.City,
            Address = result.Address,
            PricePerNight = (double)result.PricePerNight
        }));

        return response;
    }
}
