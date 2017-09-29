using RdbmsEventStore.EntityFramework;

namespace EventSourcing.PoC.Application.Events
{
    public class Event : Event<long> { }
}
