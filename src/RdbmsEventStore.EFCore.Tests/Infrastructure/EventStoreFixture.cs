using System;
using Microsoft.EntityFrameworkCore;
using RdbmsEventStore.EFCore.Tests.EventStoreTests;
using RdbmsEventStore.EFCore.Tests.TestData;
using RdbmsEventStore.EventRegistry;
using RdbmsEventStore.Serialization;

namespace RdbmsEventStore.EFCore.Tests.Infrastructure
{
    public class EventStoreFixture<TId, TStreamId, TEvent, TEventMetadata, TPersistedEvent>
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TEvent : class, TEventMetadata, IMutableEvent<TStreamId>, new()
        where TPersistedEvent : class, TEventMetadata, IPersistedEvent<TStreamId>, new()
        where TEventMetadata : class, IEventMetadata<TStreamId>
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

        public EFCoreEventStore<TId, TStreamId, TEventStoreContext, TEvent, TEventMetadata, TPersistedEvent> BuildEventStore<TEventStoreContext>(TEventStoreContext dbContext)
            where TEventStoreContext : DbContext, IEFCoreEventStoreContext<TPersistedEvent>
            => new EFCoreEventStore<TId, TStreamId, TEventStoreContext, TEvent, TEventMetadata, TPersistedEvent>(dbContext, EventFactory, WriteLock, EventSerializer);
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