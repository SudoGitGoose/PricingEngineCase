using Orleans.Hosting;
using Orleans.TestingHost;
using PricingEngineCase.Contracts;

namespace PricingEngineCase.Domain.Tests;

/// <summary>Boots a single in-memory Orleans test silo shared across a test collection.</summary>
public sealed class ClusterFixture : IDisposable
{
    public ClusterFixture()
    {
        var builder = new TestClusterBuilder(1);
        builder.AddSiloBuilderConfigurator<SiloConfigurator>();
        Cluster = builder.Build();
        Cluster.Deploy();
    }

    public TestCluster Cluster { get; }

    public void Dispose() => Cluster.Dispose();

    private sealed class SiloConfigurator : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
            => siloBuilder.AddMemoryGrainStorage(PricingEngineCaseConstants.GrainStorageName);
    }
}

[CollectionDefinition(nameof(ClusterCollection))]
public sealed class ClusterCollection : ICollectionFixture<ClusterFixture>;
