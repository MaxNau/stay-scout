using Grpc.Core;
using StayScout.Contracts.Grpc;
using SearchGrpc = StayScout.Contracts.Grpc.SearchGrpcService;

namespace StayScout.Gateway.Grpc;

public sealed class ApiGatewayGrpcService(
    SearchGrpc.SearchGrpcServiceClient searchClient)
    : SearchGrpc.SearchGrpcServiceBase
{
    public override Task<StartSearchResponse> StartSearch(
        StartSearchRequest request,
        ServerCallContext context) =>
        searchClient.StartSearchAsync(
            request,
            cancellationToken: context.CancellationToken).ResponseAsync;

    public override Task<GetSearchResponse> GetSearch(
        GetSearchRequest request,
        ServerCallContext context) =>
        searchClient.GetSearchAsync(
            request,
            cancellationToken: context.CancellationToken).ResponseAsync;
}
