using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PricingEngineCase.Contracts;

namespace PricingEngineCase.Api.Tests;

public class PricingApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PricingApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Setting_a_plan_then_pricing_a_session_returns_a_cost_breakdown()
    {
        var client = _factory.CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;
        var locationId = $"loc-{Guid.NewGuid():N}";

        var putResponse = await client.PutAsJsonAsync($"/v1/locations/{locationId}/pricing-plan", new PricingPlan
        {
            Currency = "DKK",
            EnergyPricePerKwh = 2m,
            TimePricePerMinute = 0m,
            StartFee = 0m,
            VatRate = 0.25m,
        }, cancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var sessionId = $"session-{Guid.NewGuid():N}";
        var start = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var priceResponse = await client.PostAsJsonAsync("/v1/sessions", new PriceSessionRequest
        {
            SessionId = sessionId,
            LocationId = locationId,
            StartTime = start,
            EndTime = start.AddHours(1),
            EnergyKwh = 10m,
        }, cancellationToken);

        priceResponse.EnsureSuccessStatusCode();
        var priced = await priceResponse.Content.ReadFromJsonAsync<PricedSession>(cancellationToken);

        Assert.NotNull(priced);
        Assert.Equal(20m, priced.EnergyCost);
        Assert.Equal(25m, priced.Total);
        Assert.Equal("DKK", priced.Currency);
    }

    [Fact]
    public async Task Listing_locations_returns_seeded_locations_with_distinct_plans()
    {
        var client = _factory.CreateClient();

        var locations = await client.GetFromJsonAsync<IReadOnlyList<Location>>(
            "/v1/locations", TestContext.Current.CancellationToken);

        Assert.NotNull(locations);
        Assert.Contains(locations, location => location.LocationId == "loc-copenhagen-01");
        Assert.True(
            locations.Select(location => location.Plan.EnergyPricePerKwh).Distinct().Count() > 1,
            "Seeded locations should have distinct pricing plans.");
    }

    [Fact]
    public async Task Pricing_a_session_with_end_before_start_is_rejected()
    {
        var client = _factory.CreateClient();
        var start = new DateTimeOffset(2026, 1, 1, 11, 0, 0, TimeSpan.Zero);

        var response = await client.PostAsJsonAsync("/v1/sessions", new PriceSessionRequest
        {
            SessionId = $"session-{Guid.NewGuid():N}",
            LocationId = $"loc-{Guid.NewGuid():N}",
            StartTime = start,
            EndTime = start.AddHours(-1),
            EnergyKwh = 10m,
        }, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Reading_an_unknown_session_returns_404()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/v1/sessions/missing-{Guid.NewGuid():N}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
