using System;
using Microsoft.EntityFrameworkCore;
using RdbmsEventStore.Serialization;
using Xunit;

namespace RdbmsEventStore.EFCore.Tests.Infrastructure
{
    public class EventStoreTestBase<TId, TStreamId, TEvent, TEventMetadata, TPersistedEvent> : IClassFixture<EventStoreFixture<TId, TStreamId, TEvent, TEventMetadata, TPersistedEvent>>, IDisposable
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TEventMetadata : class, IEventMetadata<TStreamId>
        where TEvent : class, TEventMetadata, IMutableEvent<TStreamId>, new()
        where TPersistedEvent : class, TEventMetadata, IPersistedEvent<TStreamId>, new()
    {
        protected readonly EventStoreFixture<TId, TStreamId, TEvent, TEventMetadata, TPersistedEvent> _fixture;
        protected readonly EFCoreContext<TStreamId, TPersistedEvent> _dbContext;

        public EventStoreTestBase(EventStoreFixture<TId, TStreamId, TEvent, TEventMetadata, TPersistedEvent> fixture)
        {
            _fixture = fixture;
            var options = new DbContextOptionsBuilder<EFCoreContext<TStreamId, TPersistedEvent>>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new EFCoreContext<TStreamId, TPersistedEvent>(options);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}