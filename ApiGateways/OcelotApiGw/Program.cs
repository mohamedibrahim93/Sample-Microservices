using Common.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Ocelot.Cache.CacheManager;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);

builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());

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

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello OcelotApiGw!");
    });
});

await app.UseOcelot();

app.Run();