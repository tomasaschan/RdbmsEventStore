using System;
using RdbmsEventStore.EventRegistry;

namespace RdbmsEventStore.EntityFramework.Tests.Infrastructure
{
    public class EventStoreFixture<TId, TStreamId, TEvent>
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TEvent : Event<TId, TStreamId>, new()
    {
        public EventStoreFixture()
        {
            EventRegistry = new AssemblyEventRegistry(typeof(TEvent), type => type.Name, type => !type.Name.StartsWith("<>"));
            EventSerializer = new DefaultEventSerializer();
            EventFactory = new DefaultEventFactory<TId, TStreamId, TEvent>(EventRegistry, EventSerializer);
            WriteLock = new WriteLock();
        }

        public IEventRegistry EventRegistry { get; }
        public IEventSerializer EventSerializer { get; }
        public DefaultEventFactory<TId, TStreamId, TEvent> EventFactory { get; }
        public IWriteLock WriteLock { get; }

        public EntityFrameworkEventStore<TId, TStreamId, EventStoreContext<TEvent>, TEvent> BuildEventStore(EventStoreContext<TEvent> dbContext)
            => new EntityFrameworkEventStore<TId, TStreamId, EventStoreContext<TEvent>, TEvent>(dbContext, EventFactory, WriteLock);
    }
}