using Orleans;
using Orleans.Runtime;
using PricingEngineCase.Contracts;

namespace PricingEngineCase.Domain.PricingPlans;

internal sealed class PricingPlanGrain : Grain, IPricingPlanGrain
{
    private readonly IPersistentState<PricingPlanGrainState> _state;

    public PricingPlanGrain(
        [PersistentState("plan", PricingEngineCaseConstants.GrainStorageName)]
        IPersistentState<PricingPlanGrainState> state)
    {
        _state = state;
    }

    public async Task SetPlanAsync(PricingPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        _state.State.Plan = plan;
        await _state.WriteStateAsync();
    }

    public Task<PricingPlan> GetPlanAsync()
        => Task.FromResult(_state.State.Plan ?? PricingPlan.Default);
}
