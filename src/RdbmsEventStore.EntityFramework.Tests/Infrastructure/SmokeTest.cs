using RdbmsEventStore.EntityFramework.Tests.TestData;
using Xunit;

namespace RdbmsEventStore.EntityFramework.Tests.Infrastructure
{
    public class SmokeTest : EventStoreTestBase<long, long, LongEvent, IEventMetadata<long>, LongLongPersistedEvent>
    {
        public SmokeTest(EventStoreFixture<long, long, LongEvent, IEventMetadata<long>, LongLongPersistedEvent> fixture, AssemblyInitializerFixture assemblyInitializer)
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