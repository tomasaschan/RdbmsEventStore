using RdbmsEventStore.EntityFramework;

namespace EventSourcing.PoC.Persistence
{
    public class EventsContext : EntityFrameworkEventStoreContext<Event>
    {
        public EventsContext() : base("name=EventSourcingPoC") { }
    }
}