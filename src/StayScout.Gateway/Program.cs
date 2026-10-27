
using StayScout.Gateway.Grpc;
using SearchGrpc = StayScout.Contracts.Grpc.SearchGrpcService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddGrpc()
    .AddJsonTranscoding();

builder.Services.AddGrpcClient<SearchGrpc.SearchGrpcServiceClient>(options =>
{
    var address = builder.Configuration["SearchService:Address"]
        ?? throw new InvalidOperationException(
            "SearchService:Address is not configured.");

    options.Address = new Uri(address);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapGrpcService<ApiGatewayGrpcService>();

app.Run();
