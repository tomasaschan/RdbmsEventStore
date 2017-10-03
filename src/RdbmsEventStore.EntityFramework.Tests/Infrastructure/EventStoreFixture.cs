using System;
using RdbmsEventStore.EntityFramework.Tests.TestData;
using RdbmsEventStore.EventRegistry;

namespace RdbmsEventStore.EntityFramework.Tests.Infrastructure
{
    public class EventStoreFixture
    {
        public EventStoreFixture()
        {
            EventRegistry = new AssemblyEventRegistry(typeof(TestEvent), type => type.Name, type => !type.Name.StartsWith("<>"));
            EventSerializer = new DefaultEventSerializer();
            EventFactory = new DefaultEventFactory<Guid, TestEvent>(EventRegistry, EventSerializer);
            WriteLock = new WriteLock();
        }

        public IEventRegistry EventRegistry { get; }
        public IEventSerializer EventSerializer { get; }
        public IEventFactory<Guid, TestEvent> EventFactory { get; }
        public IWriteLock WriteLock { get; }

        public EntityFrameworkEventStore<Guid, EventStoreContext<TestEvent>, TestEvent> BuildEventStore(EventStoreContext<TestEvent> dbContext)
            => new EntityFrameworkEventStore<Guid, EventStoreContext<TestEvent>, TestEvent>(dbContext, EventFactory, WriteLock);
    }
}