using Orleans;
using PricingEngineCase.Contracts;

namespace PricingEngineCase.Domain.PricingPlans;

/// <summary>Persisted state for <see cref="PricingPlanGrain"/>.</summary>
[GenerateSerializer, Alias("PricingPlanGrainState.v1")]
public sealed record PricingPlanGrainState
{
    /// <summary>The configured plan, or <see langword="null"/> if none has been set.</summary>
    [Id(0)] public PricingPlan? Plan { get; set; }
}
