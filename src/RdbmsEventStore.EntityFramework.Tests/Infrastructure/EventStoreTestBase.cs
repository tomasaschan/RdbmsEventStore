using System;
using RdbmsEventStore.EntityFramework.Tests.TestData;
using Xunit;

namespace RdbmsEventStore.EntityFramework.Tests.Infrastructure
{
    [Collection(nameof(InMemoryDatabaseCollection))]
    public class EventStoreTestBase :
        IClassFixture<EventStoreFixture>,
        IDisposable
    {
        protected readonly EventStoreFixture _fixture;
        protected readonly EventStoreContext<TestEvent> _dbContext;

        public EventStoreTestBase(EventStoreFixture fixture, AssemblyInitializerFixture initializer)
        {
            EffortProviderFactory.ResetDb();
            _fixture = fixture;
            _dbContext = new EventStoreContext<TestEvent>();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}