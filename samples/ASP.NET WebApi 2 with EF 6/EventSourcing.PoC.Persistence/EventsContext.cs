using RdbmsEventStore.EntityFramework;

namespace EventSourcing.PoC.Persistence
{
    public class EventsContext : EventStoreContext<Event>
    {
        public EventsContext() : base("name=EventSourcingPoC") { }
    }
}