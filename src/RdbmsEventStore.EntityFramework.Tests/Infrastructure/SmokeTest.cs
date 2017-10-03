using Xunit;

namespace RdbmsEventStore.EntityFramework.Tests.Infrastructure
{
    public class SmokeTest : EventStoreTestBase
    {
        public SmokeTest(EventStoreFixture fixture, AssemblyInitializerFixture assemblyInitializer)
            : base(fixture, assemblyInitializer)
        {
        }

        [Fact]
        public void CanInitializeEventStore()
        {
            var _ = _fixture.BuildEventStore(_dbContext);
        }
    }
}