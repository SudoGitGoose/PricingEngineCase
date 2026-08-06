using Orleans.TestingHost;
using PricingEngineCase.Contracts;
using PricingEngineCase.Domain.ChargingSessions;
using PricingEngineCase.Domain.PricingPlans;

namespace PricingEngineCase.Domain.Tests;

[Collection(nameof(ClusterCollection))]
public class ChargingSessionGrainTests
{
    private readonly TestCluster _cluster;

    public ChargingSessionGrainTests(ClusterFixture fixture) => _cluster = fixture.Cluster;

    [Fact]
    public async Task Pricing_a_session_uses_the_locations_plan()
    {
        var locationId = $"loc-{Guid.NewGuid():N}";
        var sessionId = $"session-{Guid.NewGuid():N}";

        await _cluster.GrainFactory.GetGrain<IPricingPlanGrain>(locationId).SetPlanAsync(new PricingPlan
        {
            Currency = "DKK",
            EnergyPricePerKwh = 3m,
            TimePricePerMinute = 0m,
            StartFee = 0m,
            VatRate = 0m,
        });

        var start = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var priced = await _cluster.GrainFactory.GetGrain<IChargingSessionGrain>(sessionId).PriceAsync(new PriceSessionRequest
        {
            SessionId = sessionId,
            LocationId = locationId,
            StartTime = start,
            EndTime = start.AddHours(1),
            EnergyKwh = 10m,
        });

        Assert.Equal(30m, priced.EnergyCost);
        Assert.Equal(30m, priced.Total);
        Assert.Equal("DKK", priced.Currency);
    }

    [Fact]
    public async Task Priced_session_is_persisted_and_can_be_read_back()
    {
        var sessionId = $"session-{Guid.NewGuid():N}";
        var start = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var grain = _cluster.GrainFactory.GetGrain<IChargingSessionGrain>(sessionId);

        var priced = await grain.PriceAsync(new PriceSessionRequest
        {
            SessionId = sessionId,
            LocationId = $"loc-{Guid.NewGuid():N}",
            StartTime = start,
            EndTime = start.AddMinutes(30),
            EnergyKwh = 5m,
        });

        var readBack = await grain.GetAsync();

        Assert.NotNull(readBack);
        Assert.Equal(priced.Total, readBack.Total);
    }

    [Fact]
    public async Task Unknown_session_returns_null()
    {
        var grain = _cluster.GrainFactory.GetGrain<IChargingSessionGrain>($"missing-{Guid.NewGuid():N}");

        var result = await grain.GetAsync();

        Assert.Null(result);
    }
}
