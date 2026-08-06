using Orleans;
using Orleans.Concurrency;
using PricingEngineCase.Contracts;

namespace PricingEngineCase.Domain.PricingPlans;

/// <summary>Grain that owns the pricing plan for a single location (keyed by location id).</summary>
[Alias("IPricingPlanGrain.v1")]
public interface IPricingPlanGrain : IGrainWithStringKey
{
    /// <summary>Stores the pricing plan for this location.</summary>
    Task SetPlanAsync(PricingPlan plan);

    /// <summary>Returns the location's plan, or <see cref="PricingPlan.Default"/> if none is set.</summary>
    [ReadOnly]
    Task<PricingPlan> GetPlanAsync();
}
