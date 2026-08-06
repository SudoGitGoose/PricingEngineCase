using Orleans;

namespace PricingEngineCase.Contracts;

/// <summary>
/// The pricing plan for a single location: how a charging session at that location is priced.
/// </summary>
[GenerateSerializer, Alias("PricingPlan.v1")]
public sealed record PricingPlan
{
    /// <summary>ISO currency code the plan is priced in (e.g. "DKK").</summary>
    [Id(0)] public required string Currency { get; init; }

    /// <summary>Price charged per kilowatt-hour of energy delivered.</summary>
    [Id(1)] public required decimal EnergyPricePerKwh { get; init; }

    /// <summary>Price charged per minute the session lasts.</summary>
    [Id(2)] public required decimal TimePricePerMinute { get; init; }

    /// <summary>Flat fee charged once per session.</summary>
    [Id(3)] public required decimal StartFee { get; init; }

    /// <summary>VAT rate applied to the subtotal, expressed as a fraction (e.g. 0.25 for 25%).</summary>
    [Id(4)] public required decimal VatRate { get; init; }

    /// <summary>The plan used when a location has no explicit plan configured.</summary>
    public static PricingPlan Default { get; } = new()
    {
        Currency = "DKK",
        EnergyPricePerKwh = 2.50m,
        TimePricePerMinute = 0m,
        StartFee = 0m,
        VatRate = 0.25m,
    };
}
