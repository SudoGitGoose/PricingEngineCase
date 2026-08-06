using Orleans;
using Orleans.Runtime;
using PricingEngineCase.Contracts;
using PricingEngineCase.Domain.Pricing;
using PricingEngineCase.Domain.PricingPlans;

namespace PricingEngineCase.Domain.ChargingSessions;

internal sealed class ChargingSessionGrain : Grain, IChargingSessionGrain
{
    private readonly IPersistentState<ChargingSessionGrainState> _state;

    public ChargingSessionGrain(
        [PersistentState("session", PricingEngineCaseConstants.GrainStorageName)]
        IPersistentState<ChargingSessionGrainState> state)
    {
        _state = state;
    }

    public async Task<PricedSession> PriceAsync(PriceSessionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var plan = await GrainFactory.GetGrain<IPricingPlanGrain>(request.LocationId).GetPlanAsync();

        var priced = SessionPriceCalculator.Calculate(request, plan);

        _state.State.PricedSession = priced;
        await _state.WriteStateAsync();

        return priced;
    }

    public Task<PricedSession?> GetAsync()
        => Task.FromResult(_state.State.PricedSession);
}
