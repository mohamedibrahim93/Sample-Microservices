using Common.Logging;
using Discount.API.Extensions;
using Discount.API.Repositories;
using Discount.API.Repositories.Interfaces;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

// Add services to the container.
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Discount.API", Version = "v1" });
//});

builder.Services.AddHealthChecks()
              .AddNpgSql(builder.Configuration["DatabaseSettings:ConnectionString"]);

var app = builder.Build();

app.Use(async (context, next) =>
{
    //TODO: Move to Custom Middleware
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
    //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Discount.API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MigrateDatabase<Program>();
app.Run();