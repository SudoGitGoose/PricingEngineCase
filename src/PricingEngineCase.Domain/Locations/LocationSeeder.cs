using Orleans;
using Orleans.Runtime;
using PricingEngineCase.Domain.PricingPlans;

namespace PricingEngineCase.Domain.Locations;

/// <summary>
/// Orleans startup task that seeds each well-known location's pricing plan into its grain when the
/// silo starts, so charging sessions are priced with distinct, location-specific plans.
/// </summary>
public sealed class LocationSeeder : IStartupTask
{
    private readonly IGrainFactory _grains;

    public LocationSeeder(IGrainFactory grains) => _grains = grains;

    public async Task Execute(CancellationToken cancellationToken)
    {
        foreach (var location in LocationCatalog.All)
        {
            await _grains.GetGrain<IPricingPlanGrain>(location.LocationId).SetPlanAsync(location.Plan);
        }
    }
}
