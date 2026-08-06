using Orleans;

namespace PricingEngineCase.Contracts;

/// <summary>The calculated cost breakdown for a priced charging session.</summary>
[GenerateSerializer, Alias("PricedSession.v1")]
public sealed record PricedSession
{
    /// <summary>Identifier of the priced session.</summary>
    [Id(0)] public required string SessionId { get; init; }

    /// <summary>Location the session took place at.</summary>
    [Id(1)] public required string LocationId { get; init; }

    /// <summary>Total energy delivered, in kilowatt-hours.</summary>
    [Id(2)] public required decimal EnergyKwh { get; init; }

    /// <summary>Session duration, in minutes.</summary>
    [Id(3)] public required decimal DurationMinutes { get; init; }

    /// <summary>Cost attributable to energy delivered.</summary>
    [Id(4)] public required decimal EnergyCost { get; init; }

    /// <summary>Cost attributable to session duration.</summary>
    [Id(5)] public required decimal TimeCost { get; init; }

    /// <summary>The flat start fee applied.</summary>
    [Id(6)] public required decimal StartFee { get; init; }

    /// <summary>Sum of energy, time and start fee, excluding VAT.</summary>
    [Id(7)] public required decimal SubtotalExclVat { get; init; }

    /// <summary>VAT amount applied to the subtotal.</summary>
    [Id(8)] public required decimal Vat { get; init; }

    /// <summary>Grand total, including VAT.</summary>
    [Id(9)] public required decimal Total { get; init; }

    /// <summary>Currency of all monetary amounts.</summary>
    [Id(10)] public required string Currency { get; init; }
}
