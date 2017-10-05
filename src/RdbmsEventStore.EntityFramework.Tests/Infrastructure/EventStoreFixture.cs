using System;
using System.Data.Entity;
using RdbmsEventStore.EventRegistry;

namespace RdbmsEventStore.EntityFramework.Tests.Infrastructure
{
    public class EventStoreFixture<TId, TStreamId, TEvent>
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TEvent : class, IMutableEvent<TId, TStreamId>, new()
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

        public EntityFrameworkEventStore<TId, TStreamId, TEventStoreContext, TEvent> BuildEventStore<TEventStoreContext>(TEventStoreContext dbContext)
            where TEventStoreContext : DbContext, IEventDbContext<TEvent>
            => new EntityFrameworkEventStore<TId, TStreamId, TEventStoreContext, TEvent>(dbContext, EventFactory, WriteLock);
    }
}