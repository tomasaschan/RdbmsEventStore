using System;
using System.Data.Entity;
using RdbmsEventStore.EntityFramework.Tests.EventStoreTests;
using RdbmsEventStore.EntityFramework.Tests.TestData;
using RdbmsEventStore.EventRegistry;
using RdbmsEventStore.Serialization;

namespace RdbmsEventStore.EntityFramework.Tests.Infrastructure
{
    public class EventStoreFixture<TId, TStreamId, TEvent, TEventMetadata, TPersistedEvent>
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TEvent : class, TEventMetadata, IMutableEvent<TStreamId>, new()
        where TPersistedEvent : class, TEventMetadata, IPersistedEvent<TStreamId>, new()
        where TEventMetadata : IEventMetadata<TStreamId>
    {
        public EventStoreFixture()
        {
            EventRegistry = new AssemblyEventRegistry(typeof(TEvent), type => type.Name, type => !type.Name.StartsWith("<>"));
            EventSerializer = new DefaultEventSerializer<TStreamId, TEvent, TPersistedEvent>(EventRegistry);
            EventFactory = new DefaultEventFactory<TStreamId, TEvent>();
            WriteLock = new WriteLock<TStreamId>();
        }

        public IEventRegistry EventRegistry { get; protected set; }
        public IEventSerializer<TEvent, TPersistedEvent> EventSerializer { get; protected set; }
        public IEventFactory<TStreamId, TEvent> EventFactory { get; protected set; }
        public IWriteLock<TStreamId> WriteLock { get; protected set; }

        public EntityFrameworkEventStore<TId, TStreamId, TEventStoreContext, TEvent, TEventMetadata, TPersistedEvent> BuildEventStore<TEventStoreContext>(TEventStoreContext dbContext)
            where TEventStoreContext : DbContext, IEventDbContext<TPersistedEvent>
            => new EntityFrameworkEventStore<TId, TStreamId, TEventStoreContext, TEvent, TEventMetadata, TPersistedEvent>(dbContext, EventFactory, WriteLock, EventSerializer);
    }

    public class ExtraMetaEventFactoryFixture : EventStoreFixture<long, string, ExtraMetaStringEvent, IExtraMeta, ExtraMetaLongStringPersistedEventMetadata>
    {
        public ExtraMetaEventFactoryFixture()
        {
            EventFactory = new ExtraMetaEventFactory();
            EventSerializer = new ExtraMetaEventSerializer(EventRegistry);
        }
    }
}