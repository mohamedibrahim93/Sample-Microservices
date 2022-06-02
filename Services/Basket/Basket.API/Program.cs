using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Common.Logging;
using Discount.Grpc.Protos;
using Grpc.Health.V1;
using Grpc.Net.Client;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

// Redis Configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});

// Add services to the container.
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddAutoMapper(typeof(Program));

// Grpc Configuration
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
    (o => o.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));
builder.Services.AddScoped<DiscountGrpcService>();

// MassTransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.UseHealthCheck(ctx);
    });
});
builder.Services.AddMassTransitHostedService();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration["CacheSettings:ConnectionString"], "Redis Health", HealthStatus.Degraded)
    .AddAsyncCheck("Discount.Grpc", async () =>
    {
        var channel = GrpcChannel.ForAddress(builder.Configuration["GrpcSettings:DiscountUrl"]);
        var client = new Health.HealthClient(channel);
        var response = await client.CheckAsync(new HealthCheckRequest());
        var status = response.Status;
        return status == HealthCheckResponse.Types.ServingStatus.Serving ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy(); 
    });

var app = builder.Build();

app.Use(async (context, next) =>
{
    // Do work that can write to the Response.
    LogContext.PushProperty("UserName", context.User.Identity?.Name ?? "Anonymous");
    LogContext.PushProperty("ClientIp", context.Connection.RemoteIpAddress?.ToString());
    LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString());

    await next.Invoke();
    // Do logging or other work that doesn't write to the Response.
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();