using System.Collections.Generic;

namespace RdbmsEventStore
{
    public interface IEventFactory<in TId, out TEvent> where TEvent : IEvent<TId>
    {
        IEnumerable<TEvent> Create(TId streamId, long version, params object[] payloads);
    }
}