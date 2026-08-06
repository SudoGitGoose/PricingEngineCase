using Microsoft.AspNetCore.Http.HttpResults;
using Orleans;
using PricingEngineCase.Contracts;
using PricingEngineCase.Domain.ChargingSessions;
using PricingEngineCase.Domain.Locations;
using PricingEngineCase.Domain.PricingPlans;

namespace PricingEngineCase.Api.Endpoints;

/// <summary>Maps the charging-session pricing HTTP endpoints onto the app.</summary>
public static class PricingEndpoints
{
    public static IEndpointRouteBuilder MapPricingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(PricingEngineCaseConstants.ApiBasePath);

        group.MapGet("/locations", GetLocationsAsync);
        group.MapPut("/locations/{locationId}/pricing-plan", SetPlanAsync);
        group.MapGet("/locations/{locationId}/pricing-plan", GetPlanAsync);
        group.MapPost("/sessions", PriceSessionAsync);
        group.MapGet("/sessions/{sessionId}", GetSessionAsync);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<Location>>> GetLocationsAsync(IGrainFactory grains)
    {
        var locations = new List<Location>(LocationCatalog.All.Count);
        foreach (var seed in LocationCatalog.All)
        {
            var plan = await grains.GetGrain<IPricingPlanGrain>(seed.LocationId).GetPlanAsync();
            locations.Add(seed with { Plan = plan });
        }

        return TypedResults.Ok<IReadOnlyList<Location>>(locations);
    }

    private static async Task<NoContent> SetPlanAsync(string locationId, PricingPlan plan, IGrainFactory grains)
    {
        await grains.GetGrain<IPricingPlanGrain>(locationId).SetPlanAsync(plan);
        return TypedResults.NoContent();
    }

    private static async Task<Ok<PricingPlan>> GetPlanAsync(string locationId, IGrainFactory grains)
    {
        var plan = await grains.GetGrain<IPricingPlanGrain>(locationId).GetPlanAsync();
        return TypedResults.Ok(plan);
    }

    private static async Task<Results<Ok<PricedSession>, ValidationProblem>> PriceSessionAsync(
        PriceSessionRequest request, IGrainFactory grains)
    {
        var errors = Validate(request);
        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var priced = await grains.GetGrain<IChargingSessionGrain>(request.SessionId).PriceAsync(request);
        return TypedResults.Ok(priced);
    }

    private static async Task<Results<Ok<PricedSession>, NotFound>> GetSessionAsync(string sessionId, IGrainFactory grains)
    {
        var priced = await grains.GetGrain<IChargingSessionGrain>(sessionId).GetAsync();
        return priced is null ? TypedResults.NotFound() : TypedResults.Ok(priced);
    }

    private static Dictionary<string, string[]> Validate(PriceSessionRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.SessionId))
        {
            errors[nameof(PriceSessionRequest.SessionId)] = ["Session id is required."];
        }

        if (string.IsNullOrWhiteSpace(request.LocationId))
        {
            errors[nameof(PriceSessionRequest.LocationId)] = ["Location id is required."];
        }

        if (request.EndTime <= request.StartTime)
        {
            errors[nameof(PriceSessionRequest.EndTime)] = ["End time must be after start time."];
        }

        if (request.EnergyKwh < 0m)
        {
            errors[nameof(PriceSessionRequest.EnergyKwh)] = ["Energy consumed cannot be negative."];
        }

        return errors;
    }
}
