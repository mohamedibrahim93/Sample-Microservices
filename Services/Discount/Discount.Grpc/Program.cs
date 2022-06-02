using Common.Logging;
using Discount.Grpc.Extensions;
using Discount.Grpc.Repositories;
using Discount.Grpc.Repositories.Interfaces;
using Discount.Grpc.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddGrpc();

builder.Services.AddGrpcHealthChecks()
    //.AddNpgSql(builder.Configuration["DatabaseSettings:ConnectionString"])
    .AddCheck("Sample", () => HealthCheckResult.Healthy());

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

app.MapGrpcService<DiscountService>();
app.MapGrpcHealthChecksService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.MigrateDatabase<Program>();
app.Run();