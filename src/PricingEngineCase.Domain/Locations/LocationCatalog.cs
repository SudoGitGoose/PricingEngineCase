using PricingEngineCase.Contracts;

namespace PricingEngineCase.Domain.Locations;

/// <summary>
/// The set of well-known charging locations seeded on startup, each with a distinct pricing plan.
/// Acts as the single source of truth for the locations a caller can choose from.
/// </summary>
public static class LocationCatalog
{
    /// <summary>All seeded locations, in display order.</summary>
    public static IReadOnlyList<Location> All { get; } =
    [
        new Location
        {
            LocationId = "loc-copenhagen-01",
            Name = "Copenhagen – City Center",
            Plan = new PricingPlan
            {
                Currency = "DKK",
                EnergyPricePerKwh = 2.95m,
                TimePricePerMinute = 0m,
                StartFee = 0m,
                VatRate = 0.25m,
            },
        },
        new Location
        {
            LocationId = "loc-aarhus-01",
            Name = "Aarhus – Harbor",
            Plan = new PricingPlan
            {
                Currency = "DKK",
                EnergyPricePerKwh = 2.20m,
                TimePricePerMinute = 0.25m,
                StartFee = 5m,
                VatRate = 0.25m,
            },
        },
        new Location
        {
            LocationId = "loc-odense-01",
            Name = "Odense – Drejebænken",
            Plan = new PricingPlan
            {
                Currency = "DKK",
                EnergyPricePerKwh = 1.75m,
                TimePricePerMinute = 0m,
                StartFee = 10m,
                VatRate = 0.25m,
            },
        },
    ];
}
