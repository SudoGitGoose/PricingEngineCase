using PricingEngineCase.Contracts;

namespace PricingEngineCase.Domain.Pricing;

/// <summary>
/// Pure pricing logic: turns a charging session and a pricing plan into a cost breakdown.
/// Deliberately free of Orleans and I/O so it is trivial to unit-test.
/// </summary>
public static class SessionPriceCalculator
{
    /// <summary>Calculates the cost breakdown for <paramref name="request"/> using <paramref name="plan"/>.</summary>
    public static PricedSession Calculate(PriceSessionRequest request, PricingPlan plan)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(plan);

        var durationMinutes = (decimal)(request.EndTime - request.StartTime).TotalMinutes;

        var energyCost = Round(request.EnergyKwh * plan.EnergyPricePerKwh);
        var timeCost = Round(durationMinutes * plan.TimePricePerMinute);
        var startFee = Round(plan.StartFee);

        var subtotal = energyCost + timeCost + startFee;
        var vat = Round(subtotal * plan.VatRate);
        var total = subtotal + vat;

        return new PricedSession
        {
            SessionId = request.SessionId,
            LocationId = request.LocationId,
            EnergyKwh = request.EnergyKwh,
            DurationMinutes = Round(durationMinutes),
            EnergyCost = energyCost,
            TimeCost = timeCost,
            StartFee = startFee,
            SubtotalExclVat = subtotal,
            Vat = vat,
            Total = total,
            Currency = plan.Currency,
        };
    }

    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
