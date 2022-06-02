using Common.Logging;
using EventBus.Messages.Common;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add services to the container.
builder.Services.AddScoped<BasketCheckoutConsumer>();
builder.Services.AddScoped<GeneralConsumer>();
builder.Services.AddAutoMapper(typeof(Program));

// MassTransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BasketCheckoutConsumer>();
    config.AddConsumer<GeneralConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.UseHealthCheck(ctx);

        cfg.ReceiveEndpoint(c =>
        {
            c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
            c.ConfigureConsumer<GeneralConsumer>(ctx);
        });
        //cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
        //{
        //    c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
        //});
    });
});
builder.Services.AddMassTransitHostedService();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<OrderContext>();

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

app.MigrateDatabase<OrderContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderContextSeed>>();
    if (logger != null)
        OrderContextSeed
            .SeedAsync(context, logger)
            .Wait();
}).Run();
