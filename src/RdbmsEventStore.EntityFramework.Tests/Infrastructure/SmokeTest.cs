using RdbmsEventStore.EntityFramework.Tests.TestData;
using Xunit;

namespace RdbmsEventStore.EntityFramework.Tests.Infrastructure
{
    public class SmokeTest : EventStoreTestBase<long, long, LongLongEvent>
    {
        public SmokeTest(EventStoreFixture<long, long, LongLongEvent> fixture, AssemblyInitializerFixture assemblyInitializer)
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