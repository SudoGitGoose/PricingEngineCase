using Orleans.Hosting;
using PricingEngineCase.Api.Endpoints;
using PricingEngineCase.Contracts;
using PricingEngineCase.Domain.Locations;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Co-host an Orleans silo in-process. Everything is local + in-memory: no Azure, no clustering
// backend, no persistence store. This mirrors how the real engine co-hosts its silo in the Host
// process, minus all the cloud wiring.
builder.Host.UseOrleans(silo =>
{
    silo.UseLocalhostClustering();
    silo.AddMemoryGrainStorage(PricingEngineCaseConstants.GrainStorageName);

    // Seed the well-known locations' pricing plans once the silo is active so sessions are
    // priced with distinct, location-specific plans instead of the fallback default.
    silo.AddStartupTask<LocationSeeder>();
});

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options => options.AddPolicy(
    PricingEngineCaseConstants.WebCorsPolicy,
    policy => policy
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors(PricingEngineCaseConstants.WebCorsPolicy);

app.MapOpenApi();
app.MapScalarApiReference();
app.MapPricingEndpoints();

app.MapGet("/", () => Results.Ok("PricingEngineCase API - explore at /scalar"));

await app.RunAsync();

/// <summary>Exposed so the integration test project can use <c>WebApplicationFactory&lt;Program&gt;</c>.</summary>
public partial class Program { }
