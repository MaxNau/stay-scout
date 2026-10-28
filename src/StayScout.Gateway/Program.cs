using Grpc.Core;
using Grpc.Net.Client.Configuration;
using StayScout.Gateway.Endpoints;
using StayScout.Gateway.ExceptionHandling;
using SearchGrpc = StayScout.Contracts.Grpc.SearchGrpcService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GrpcExceptionHandler>();
builder.Services.AddExceptionHandler<UnhandledExceptionHandler>();

builder.Services.AddGrpcClient<SearchGrpc.SearchGrpcServiceClient>(options =>
{
    var address = builder.Configuration["SearchService:Address"]
        ?? throw new InvalidOperationException(
            "SearchService:Address is not configured.");

    options.Address = new Uri(address);
})
.ConfigureChannel(options =>
   {
       options.ServiceConfig = new ServiceConfig
       {
           MethodConfigs =
        {
            new MethodConfig
            {
                Names =
                {
                    new MethodName
                    {
                        Service = "stayscout.search.SearchGrpcService",
                        Method = "GetSearch"
                    }
                },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 3,
                    InitialBackoff = TimeSpan.FromMilliseconds(200),
                    MaxBackoff = TimeSpan.FromSeconds(1),
                    BackoffMultiplier = 2,
                    RetryableStatusCodes =
                    {
                        StatusCode.Unavailable
                    }
                }
            }
        }
       };
   });

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapSearchEndpoints();

app.Run();
