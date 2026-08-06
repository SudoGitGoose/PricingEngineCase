using Orleans;

namespace PricingEngineCase.Contracts;

/// <summary>A request to price a single charging session.</summary>
[GenerateSerializer, Alias("PriceSessionRequest.v1")]
public sealed record PriceSessionRequest
{
    /// <summary>Unique identifier of the charging session.</summary>
    [Id(0)] public required string SessionId { get; init; }

    /// <summary>Location the session took place at; selects the pricing plan.</summary>
    [Id(1)] public required string LocationId { get; init; }

    /// <summary>When the session started.</summary>
    [Id(2)] public required DateTimeOffset StartTime { get; init; }

    /// <summary>When the session ended.</summary>
    [Id(3)] public required DateTimeOffset EndTime { get; init; }

    /// <summary>Total energy delivered during the session, in kilowatt-hours.</summary>
    [Id(4)] public required decimal EnergyKwh { get; init; }
}
