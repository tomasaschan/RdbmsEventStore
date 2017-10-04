using System;
using Xunit;

namespace RdbmsEventStore.EntityFramework.Tests.Infrastructure
{
    [Collection(nameof(InMemoryDatabaseCollection))]
    public class EventStoreTestBase<TId, TStreamId, TEvent> : IClassFixture<EventStoreFixture<TId, TStreamId, TEvent>>, IDisposable
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TEvent : Event<TId, TStreamId>, new()
    {
        protected readonly EventStoreFixture<TId, TStreamId, TEvent> _fixture;
        protected readonly EventStoreContext<TEvent> _dbContext;

        public EventStoreTestBase(EventStoreFixture<TId, TStreamId, TEvent> fixture, AssemblyInitializerFixture initializer)
        {
            EffortProviderFactory.ResetDb();
            _fixture = fixture;
            _dbContext = new EventStoreContext<TEvent>();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}