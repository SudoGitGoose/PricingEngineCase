using Orleans;
using PricingEngineCase.Contracts;

namespace PricingEngineCase.Domain.ChargingSessions;

/// <summary>Persisted state for <see cref="ChargingSessionGrain"/>.</summary>
[GenerateSerializer, Alias("ChargingSessionGrainState.v1")]
public sealed record ChargingSessionGrainState
{
    /// <summary>The priced session, or <see langword="null"/> until the session has been priced.</summary>
    [Id(0)] public PricedSession? PricedSession { get; set; }
}
