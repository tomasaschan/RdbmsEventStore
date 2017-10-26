using Xunit;

namespace RdbmsEventStore.EntityFramework.Tests.Infrastructure
{
    public class AssemblyInitializerFixture
    {
        public AssemblyInitializerFixture()
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
        }
    }

    [CollectionDefinition(nameof(InMemoryDatabaseCollection), DisableParallelization = true)]
    public class InMemoryDatabaseCollection : ICollectionFixture<AssemblyInitializerFixture> { }
}