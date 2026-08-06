using Orleans;
using Orleans.Concurrency;
using PricingEngineCase.Contracts;

namespace PricingEngineCase.Domain.ChargingSessions;

/// <summary>Grain that prices and stores a single charging session (keyed by session id).</summary>
[Alias("IChargingSessionGrain.v1")]
public interface IChargingSessionGrain : IGrainWithStringKey
{
    /// <summary>Prices the session against its location's plan, stores the result, and returns it.</summary>
    Task<PricedSession> PriceAsync(PriceSessionRequest request);

    /// <summary>Returns the previously priced session, or <see langword="null"/> if it has not been priced.</summary>
    [ReadOnly]
    Task<PricedSession?> GetAsync();
}
