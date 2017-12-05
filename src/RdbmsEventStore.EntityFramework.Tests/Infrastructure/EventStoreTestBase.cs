using System;
using RdbmsEventStore.Serialization;
using Xunit;

namespace RdbmsEventStore.EntityFramework.Tests.Infrastructure
{
    [Collection(nameof(InMemoryDatabaseCollection))]
    public class EventStoreTestBase<TId, TStreamId, TEvent, TEventMetadata, TPersistedEvent> : IClassFixture<EventStoreFixture<TId, TStreamId, TEvent, TEventMetadata, TPersistedEvent>>, IDisposable
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TEventMetadata : class, IEventMetadata<TStreamId>
        where TEvent : class, TEventMetadata, IMutableEvent<TStreamId>, new()
        where TPersistedEvent : class, TEventMetadata, IPersistedEvent<TStreamId>, new()
    {
        protected readonly EventStoreFixture<TId, TStreamId, TEvent, TEventMetadata, TPersistedEvent> _fixture;
        protected readonly EntityFrameworkEventStoreContext<TPersistedEvent> _dbContext;

        public EventStoreTestBase(EventStoreFixture<TId, TStreamId, TEvent, TEventMetadata, TPersistedEvent> fixture, AssemblyInitializerFixture initializer)
        {
            EffortProviderFactory.ResetDb();
            _fixture = fixture;
            _dbContext = new EntityFrameworkEventStoreContext<TPersistedEvent>();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}