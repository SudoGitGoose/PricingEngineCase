using PricingEngineCase.Contracts;
using PricingEngineCase.Domain.Pricing;

namespace PricingEngineCase.Domain.Tests;

public class SessionPriceCalculatorTests
{
    private static PricingPlan Plan(
        decimal energy = 2m,
        decimal time = 0m,
        decimal startFee = 0m,
        decimal vat = 0.25m,
        string currency = "DKK")
        => new()
        {
            Currency = currency,
            EnergyPricePerKwh = energy,
            TimePricePerMinute = time,
            StartFee = startFee,
            VatRate = vat,
        };

    private static PriceSessionRequest Session(decimal kwh = 10m, double minutes = 60d)
    {
        var start = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        return new PriceSessionRequest
        {
            SessionId = "s1",
            LocationId = "loc1",
            StartTime = start,
            EndTime = start.AddMinutes(minutes),
            EnergyKwh = kwh,
        };
    }

    [Fact]
    public void Energy_is_charged_per_kwh()
    {
        var result = SessionPriceCalculator.Calculate(Session(kwh: 10m), Plan(energy: 2m, vat: 0m));

        Assert.Equal(20m, result.EnergyCost);
        Assert.Equal(0m, result.TimeCost);
        Assert.Equal(20m, result.SubtotalExclVat);
        Assert.Equal(20m, result.Total);
    }

    [Fact]
    public void Time_is_charged_per_minute()
    {
        var result = SessionPriceCalculator.Calculate(Session(kwh: 0m, minutes: 30d), Plan(energy: 0m, time: 0.5m, vat: 0m));

        Assert.Equal(30m, result.DurationMinutes);
        Assert.Equal(15m, result.TimeCost);
    }

    [Fact]
    public void Start_fee_is_added_to_the_subtotal()
    {
        var result = SessionPriceCalculator.Calculate(Session(kwh: 10m), Plan(energy: 2m, startFee: 5m, vat: 0m));

        Assert.Equal(5m, result.StartFee);
        Assert.Equal(25m, result.SubtotalExclVat);
    }

    [Fact]
    public void Vat_is_applied_to_the_subtotal()
    {
        var result = SessionPriceCalculator.Calculate(Session(kwh: 10m), Plan(energy: 2m, vat: 0.25m));

        Assert.Equal(20m, result.SubtotalExclVat);
        Assert.Equal(5m, result.Vat);
        Assert.Equal(25m, result.Total);
    }

    [Fact]
    public void Monetary_amounts_are_rounded_to_two_decimals()
    {
        // 3.333 kWh * 1.111 DKK/kWh = 3.702963 -> 3.70
        var result = SessionPriceCalculator.Calculate(Session(kwh: 3.333m), Plan(energy: 1.111m, vat: 0m));

        Assert.Equal(3.70m, result.EnergyCost);
        Assert.Equal(3.70m, result.Total);
    }

    [Fact]
    public void Currency_flows_from_the_plan()
    {
        var result = SessionPriceCalculator.Calculate(Session(), Plan(currency: "EUR"));

        Assert.Equal("EUR", result.Currency);
    }
}
