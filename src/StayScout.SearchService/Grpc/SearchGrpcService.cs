using Grpc.Core;
using StayScout.Contracts.Grpc;
using SearchGrpc = StayScout.Contracts.Grpc.SearchGrpcService;

namespace StayScout.SearchService.Grpc;

public sealed class SearchGrpcService : SearchGrpc.SearchGrpcServiceBase
{
    public override Task<StartSearchResponse> StartSearch(
        StartSearchRequest request,
        ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Query is required."));
        }

        // TODO: Start the search through the application service and return its ID.

        throw new RpcException(
            new Status(StatusCode.Unimplemented, "Not implemented."));
    }

    public override Task<GetSearchResponse> GetSearch(
        GetSearchRequest request,
        ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.SearchId))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Search ID is required."));
        }

        // TODO: Retrieve the search through the application service and map its state and results.
        // TODO: Return NotFound if the search does not exist.

        throw new RpcException(
            new Status(StatusCode.Unimplemented, "Not implemented."));
    }
}
