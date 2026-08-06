using Orleans;

namespace PricingEngineCase.Contracts;

/// <summary>
/// A selectable charging location and the pricing plan applied to sessions there.
/// </summary>
[GenerateSerializer, Alias("Location.v1")]
public sealed record Location
{
    /// <summary>Stable identifier of the location; selects the pricing plan.</summary>
    [Id(0)] public required string LocationId { get; init; }

    /// <summary>Human-friendly name shown in the UI.</summary>
    [Id(1)] public required string Name { get; init; }

    /// <summary>The pricing plan applied to charging sessions at this location.</summary>
    [Id(2)] public required PricingPlan Plan { get; init; }
}
