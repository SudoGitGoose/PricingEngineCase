namespace PricingEngineCase.Contracts;

/// <summary>
/// Centralised well-known strings shared across the solution (storage provider names,
/// route bases, policy names). Mirrors the real engine's <c>PricingEngineConstants</c>.
/// </summary>
public static class PricingEngineCaseConstants
{
    /// <summary>Name of the (in-memory) Orleans grain storage provider.</summary>
    public const string GrainStorageName = "pricing-store";

    /// <summary>Base path for the versioned HTTP API.</summary>
    public const string ApiBasePath = "/v1";

    /// <summary>CORS policy name for the local web frontend.</summary>
    public const string WebCorsPolicy = "web";
}
